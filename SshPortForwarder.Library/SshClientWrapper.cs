using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace SshPortForwarder
{
    public class SshClientWrapper : IDisposable
    {
        private readonly ILogger logger;
        private readonly RetryPolicy retryPolicy;
        private readonly SshSettings settings;
        private readonly SshClient sshClient;

        public SshClientWrapper(IOptionsMonitor<SshSettings> settingsMonitor, ILogger<SshClientWrapper> logger)
        {
            this.logger = logger;
            settings = settingsMonitor.CurrentValue;
            sshClient = CreateSshClient();
            sshClient.ErrorOccurred += SshClient_ErrorOccurred;
            sshClient.HostKeyReceived += SshClient_HostKeyReceived;

            retryPolicy = Policy
                .Handle<AggregateException>()
                .WaitAndRetryForever(
                    retryAttempt => TimeSpan.FromSeconds(settings.ReconnectAfterSeconds),
                    (exception, sleepDuration) => logger?.LogWarning($"Failed to connect to SSH server: {exception.Message}\r\n" +
                        $"Reconnecting in {sleepDuration.TotalSeconds} seconds..."));

            Connect();

            if (!sshClient.ForwardedPorts.Any(p => p.IsStarted))
            {
                logger?.LogInformation($"No ports being forwarded - disconnecting from {sshClient.ConnectionInfo.Host}:{sshClient.ConnectionInfo.Port}...");
                Dispose();
            }
        }

        private void Connect()
        {
            retryPolicy.Execute(() =>
            {
                try
                {
                    logger?.LogInformation($"Connecting to {sshClient.ConnectionInfo.Host}:{sshClient.ConnectionInfo.Port}...");
                    sshClient.Connect();
                    logger?.LogInformation($"Connected.");

                    foreach (PortForwardingSettings portForwardingSettings in settings.ForwardedPorts)
                    {
                        ForwardedPortLocal forwardedPortLocal = new ForwardedPortLocal(portForwardingSettings.LocalHost, portForwardingSettings.LocalPort,
                            portForwardingSettings.RemoteHost, portForwardingSettings.RemotePort);
                        sshClient.AddForwardedPort(forwardedPortLocal);
                        logger?.LogInformation($"Starting port forwarding from {forwardedPortLocal.BoundHost}:{forwardedPortLocal.BoundPort} " +
                            $"to server's {forwardedPortLocal.Host}:{forwardedPortLocal.Port}...");

                        try
                        {
                            forwardedPortLocal.Start();
                        }
                        catch (SocketException e)
                        when (e.SocketErrorCode == SocketError.AddressAlreadyInUse && portForwardingSettings.IgnorePortInUseError)
                        {
                            logger?.LogWarning($"Port forwarding from {forwardedPortLocal.BoundHost}:{forwardedPortLocal.BoundPort} " +
                                $"to server's {forwardedPortLocal.Host}:{forwardedPortLocal.Port} could not be started: {e.Message}.");
                        }
                    }
                }
                catch (Exception exception)
                {
                    logger?.LogWarning($"Failed to forward ports: {exception}");
                    StopPortForwarding();
                    throw;
                }
            });

            logger?.LogInformation($"All port forwarding started.");
        }

        private void SshClient_ErrorOccurred(object sender, ExceptionEventArgs e)
        {
            if (sshClient.IsConnected)
            {
                logger?.LogWarning($"Error occurred: {e.Exception}");
            }
            else
            {
                logger?.LogWarning($"Connection to SSH server lost: {e.Exception}");
                StopPortForwarding();
                Connect();
            }
        }

        private void StopPortForwarding()
        {
            foreach (ForwardedPortLocal forwardedPortLocal in sshClient.ForwardedPorts.ToArray())
            {
                logger?.LogInformation($"Stopping port forwarding from {forwardedPortLocal.BoundHost}:{forwardedPortLocal.BoundPort} " +
                    $"to server's {forwardedPortLocal.Host}:{forwardedPortLocal.Port}...");

                sshClient.RemoveForwardedPort(forwardedPortLocal);
            }

            logger?.LogInformation($"All port forwarding stopped.");
        }

        private SshClient CreateSshClient()
        {
            List<AuthenticationMethod> authenticationMethods = new List<AuthenticationMethod>();

            if (settings.PrivateKey != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                {
                    streamWriter.Write(settings.PrivateKey);
                    streamWriter.Flush();
                    memoryStream.Position = 0;
                    authenticationMethods.Add(new PrivateKeyAuthenticationMethod(settings.Username, new PrivateKeyFile(memoryStream, settings.PrivateKeyPassPhrase)));
                }
            }

            if (settings.PrivateKeyFilePath != null)
            {
                authenticationMethods.Add(new PrivateKeyAuthenticationMethod(settings.Username, new PrivateKeyFile(settings.PrivateKeyFilePath, settings.PrivateKeyPassPhrase)));
            }

            if (settings.Password != null)
            {
                authenticationMethods.Add(new PasswordAuthenticationMethod(settings.Username, settings.Password));
            }

            ConnectionInfo connectionInfo = new ConnectionInfo(settings.Host, settings.Port, settings.Username,
                settings.ProxyType, settings.ProxyHost, settings.ProxyPort, settings.ProxyUsername, settings.ProxyPassword,
                authenticationMethods.ToArray());

            return new SshClient(connectionInfo);
        }

        private void SshClient_HostKeyReceived(object sender, HostKeyEventArgs e)
        {
            byte[] expectedHostKey = Convert.FromBase64String(settings.ExpectedHostKeyBase64);

            if (!e.HostKey.SequenceEqual(expectedHostKey))
            {
                e.CanTrust = false;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ConnectionInfo connectionInfo = sshClient.ConnectionInfo;
                    sshClient.Dispose();
                    logger?.LogInformation($"Disconnected from {connectionInfo.Host}:{connectionInfo.Port}.");
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}

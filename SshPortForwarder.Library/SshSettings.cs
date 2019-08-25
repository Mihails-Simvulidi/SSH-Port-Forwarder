using Renci.SshNet;
using System.ComponentModel.DataAnnotations;

namespace SshPortForwarder
{
    /// <summary>
    /// Configuration of SshClientWrapper.
    /// </summary>
    public class SshSettings
    {
        /// <summary>
        /// SSH server's hostname or IP address.
        /// </summary>
        [Required]
        public string Host { get; set; }

        /// <summary>
        /// SSH server's port. Default: 22.
        /// </summary>
        public int Port { get; set; } = 22;

        /// <summary>
        /// SSH server's username.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// SSH server's public key. Needed to verify server's identity.
        /// It's a Base64 encoded string which can be located in ~/.ssh/known_hosts after first succesful connection to the server.
        /// </summary>
        [Required]
        public string ExpectedHostKeyBase64 { get; set; }

        /// <summary>
        /// Specifies the type of proxy SSH client will use to connect to server.
        /// </summary>
        public ProxyTypes ProxyType { get; set; }

        /// <summary>
        /// Proxy server's hostname or IP address.
        /// </summary>
        public string ProxyHost { get; set; }

        /// <summary>
        /// Proxy server's port.
        /// </summary>
        public int ProxyPort { get; set; }

        /// <summary>
        /// Proxy server's username.
        /// </summary>
        public string ProxyUsername { get; set; }

        /// <summary>
        /// Proxy server's password.
        /// </summary>
        public string ProxyPassword { get; set; }

        /// <summary>
        /// Authentication option 1: using password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Authentication option 2: using private key stored in configuration.
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// Authentication option 3: using path of a file containing private key.
        /// </summary>
        public string PrivateKeyFilePath { get; set; }

        /// <summary>
        /// Passphrase for private key authentication.
        /// </summary>
        public string PrivateKeyPassPhrase { get; set; }

        /// <summary>
        /// If connection interrupts, try reconnecting every X seconds. Default: 15 seconds.
        /// </summary>
        public int ReconnectAfterSeconds { get; set; } = 15;

        /// <summary>
        /// Collection of ports to be forwarded.
        /// </summary>
        [MinLength(1)]
        [Required]
        public PortForwardingSettings[] ForwardedPorts { get; set; }
    }
}

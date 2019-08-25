# SSH-Port-Forwarder
**SSH-Port-Forwarder** is a .NET Standard library implementing *AddSshTunnel* extension method for the interface *Microsoft.Extensions.DependencyInjection.IServiceCollection*. *AddSshTunnel* establishes SSH tunnel using connection info stored in the app configuration. It can be used, for example, to establish secure connection to the database in ASP.NET Core applications. It uses [SSH.NET library](https://github.com/sshnet/SSH.NET/) internally.

[![Version](https://img.shields.io/nuget/vpre/SshPortForwarder.svg)](https://www.nuget.org/packages/SshPortForwarder)

## Usage

To use in an ASP.NET application, add the following code to the *Startup.ConfigureServices* method:
```cs
services.AddSshTunnel(configuration.GetSection("SshTunnel"));
```

Then add a constructor parameter `SshClientWrapper _` in a class where you will use a tunneled connection (for example, in a class derived from *Microsoft.EntityFrameworkCore.DbContext*).

Then add a configuration, sample:
```json
{
  "SshTunnel": {
    "ExpectedHostKeyBase64": "AAAAE2VjZHNhLXNoYTItbmlzdHAyNTYAAAAIbmlzdHAyNTYAAABBBIE154JCCcw7PeuX/z2MiIX0u9BFeVpn4ZDUGVZVzUZcsFKfrC01vQAw/fzns7u6LfoxjuNgL6ZXKdcZY/AtZNU=",
    "ForwardedPorts": [
      {
        "IgnorePortInUseError": true,
        "LocalPort": 3306,
        "RemotePort": 3306
      }
    ],
    "Host": "sshserver.net",
    "PrivateKeyFilePath": "C:\\Users\\Username\\.ssh\\id_rsa",
    "Username": "username"
  }
}
```

## Configuration options

| Name | Description |
| --- | --- |
| Host | SSH server's hostname or IP address. Required. |
| Port | SSH server's port. Default: 22. |
| Username | SSH server's username. Required. |
| ExpectedHostKeyBase64 | SSH server's host key. Used to verify that the SSH server is legitimate. After you first connected to SSH server via OpenSSH command line, you will find a value for this setting in *~/.ssh/known_hosts* in your user profile folder in the line having your SSH server's host name or IP address. |
| ProxyType | Specifies the type of proxy SSH client will use to connect to server. Available options: None, Socks4, Socks5, Http. Default: None. |
| ProxyHost | Proxy server's hostname or IP address. |
| ProxyPort | Proxy server's port. |
| ProxyUsername | Proxy server's username. |
| ProxyPassword | Proxy server's password. |
| Password | Authentication option 1: using password. |
| PrivateKey | Authentication option 2: using private key stored in configuration. |
| PrivateKeyFilePath | Authentication option 3: using path of a file containing private key. |
| PrivateKeyPassPhrase | Passphrase for private key authentication. |
| ReconnectAfterSeconds | If connection interrupts, try reconnecting every X seconds. Default: 15 seconds. |
| ForwardedPorts | Collection of ports to be forwarded. Required. |

### **ForwardedPorts** configuration options

| Name | Description |
| --- | --- |
| LocalHost | Local IP address which will accept connections. Default: 127.0.0.1. |
| LocalPort | Local port which will accept connections. |
| RemoteHost | Remote host in SSH server's network to which incoming connections will be forwarded. Default: 127.0.0.1. |
| RemotePort | Remote port to which incoming connections will be forwarded. |
| IgnorePortInUseError | Proceed if port is already busy. Default: false. |

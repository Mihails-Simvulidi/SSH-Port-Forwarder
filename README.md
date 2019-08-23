# SshPortForwarder
**SshPortForwarder** is a .NET Standard library implementing *Microsoft.Extensions.DependencyInjection.IServiceCollection.AddSshTunnel* extension method which establishes SSH tunnel using connection info stored in the app configuration. It can be used, for example, to establish secure connection to the database in ASP.NET Core applications. It uses [SSH.NET library](https://github.com/sshnet/SSH.NET/) internally.

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

# SSH-Port-Forwarder
**SSH-Port-Forwarder** is a .NET Standard library implementing *Microsoft.Extensions.DependencyInjection.IServiceCollection.AddSshTunnel* extension method which establishes SSH tunnel using connection info stored in the app configuration. It can be used, for example, to establish secure connection to the database in ASP.NET Core applications.

Sample configuration:
```
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

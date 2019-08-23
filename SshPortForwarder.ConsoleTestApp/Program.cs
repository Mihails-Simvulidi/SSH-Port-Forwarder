using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace SshPortForwarder.ConsoleTestApp
{
    class Program
    {
        static void Main()
        {
            IConfigurationRoot configurationRoot = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            try
            {
                using (ServiceProvider serviceProvider = new ServiceCollection()
                    .AddSshTunnel(configurationRoot.GetSection("SshTunnel"))
                    .AddLogging(loggingBuilder => loggingBuilder.AddConsole())
                    .AddSingleton<TestApp>()
                    .BuildServiceProvider())
                {
                    TestApp testApp = serviceProvider.GetRequiredService<TestApp>();
                    testApp.RunAsync().Wait();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
            }
        }
    }
}

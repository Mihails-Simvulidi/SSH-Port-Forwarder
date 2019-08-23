using System;
using System.Threading.Tasks;

namespace SshPortForwarder.ConsoleTestApp
{
    class TestApp
    {
        public TestApp(SshClientWrapper _)
        {
        }

        public async Task RunAsync()
        {
            const int secondsDelay = 100;
            Console.WriteLine($"Waiting for {secondsDelay} seconds...");
            await Task.Delay(secondsDelay * 1000);
        }
    }
}

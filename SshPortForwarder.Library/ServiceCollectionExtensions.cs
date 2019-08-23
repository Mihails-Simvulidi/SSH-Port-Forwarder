using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SshPortForwarder
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers singleton SshPortForwarder instance using provided configuration.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddSshTunnel(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddOptions<SshSettings>()
                .Bind(configuration)
                .ValidateDataAnnotations();

            return serviceCollection
                .AddSingleton<SshClientWrapper>();
        }
    }
}

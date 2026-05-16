using Microsoft.Extensions.DependencyInjection;
using NexusForever.API.Configuration.Model;

namespace NexusForever.API.Account.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAccountAPIClient(this IServiceCollection sc, APIConfig configuration)
        {
            sc.AddHttpClient<AccountAPIClient>(c => c.BaseAddress = new Uri(configuration.Host));
            return sc;
        }
    }
}

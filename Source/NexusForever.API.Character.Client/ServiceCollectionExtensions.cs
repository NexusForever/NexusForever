using Microsoft.Extensions.DependencyInjection;
using NexusForever.API.Configuration.Model;

namespace NexusForever.API.Character.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCharacterAPIClient(this IServiceCollection sc, APIConfig configuration)
        {
            sc.AddHttpClient<CharacterAPIClient>(c => c.BaseAddress = new Uri(configuration.Host));
            return sc;
        }
    }
}

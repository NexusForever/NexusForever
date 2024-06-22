using Microsoft.Extensions.DependencyInjection;
using NexusForever.Network.Auth.Message;

namespace NexusForever.Network.Auth
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNetworkAuth(this IServiceCollection sc)
        {
            sc.AddNetworkAuthMessage();
        }
    }
}

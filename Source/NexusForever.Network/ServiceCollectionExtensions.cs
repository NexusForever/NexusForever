using Microsoft.Extensions.DependencyInjection;
using NexusForever.Network.Message;
using NexusForever.Network.Session;
using NexusForever.Shared;

namespace NexusForever.Network
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNetworkConnectivity<TInterface, TImplementation>(this IServiceCollection sc)
            where TInterface : class, INetworkSession
            where TImplementation : class, TInterface
        {
            sc.AddTransientFactory<TInterface, TImplementation>();
            sc.AddTransient<IConnectionListener<TInterface>, ConnectionListener<TInterface>>();
            sc.AddSingleton<INetworkManager<TInterface>, NetworkManager<TInterface>>();
        }

        public static void AddNetwork(this IServiceCollection sc)
        {
            sc.AddNetworkMessage();

            sc.AddSingleton<IMessageManager, MessageManager>();
        }
    }
}

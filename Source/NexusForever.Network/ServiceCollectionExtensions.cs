using Microsoft.Extensions.DependencyInjection;
using NexusForever.Network.Message;
using NexusForever.Shared;

namespace NexusForever.Network
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNetwork<T>(this IServiceCollection sc) where T : INetworkSession, new()
        {
            sc.AddNetworkMessage();

            sc.AddSingletonLegacy<IMessageManager, MessageManager>();
            sc.AddSingletonLegacy<INetworkManager<T>, NetworkManager<T>>();
        }
    }
}

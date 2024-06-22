using Microsoft.Extensions.DependencyInjection;
using NexusForever.Network.World;
using NexusForever.WorldServer.Network.Message.Handler;

namespace NexusForever.WorldServer.Network
{
    public static class ServiceCollectionExtensions
    {
        public static void AddWorldNetwork(this IServiceCollection sc)
        {
            sc.AddNetworkWorld();
            sc.AddWorldNetworkMessageHandler();
        }
    }
}

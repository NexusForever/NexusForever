using Microsoft.Extensions.DependencyInjection;
using NexusForever.Network.World.Chat;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message;
using NexusForever.Shared;

namespace NexusForever.Network.World
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNetworkWorld(this IServiceCollection sc)
        {
            sc.AddNetworkWorldChat();
            sc.AddNetworkWorldMessage();

            sc.AddSingletonLegacy<IEntityCommandManager, EntityCommandManager>();
        }
    }
}

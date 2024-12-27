using Microsoft.Extensions.DependencyInjection;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message;
using NexusForever.Network.World.Social;
using NexusForever.Shared;

namespace NexusForever.Network.World
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNetworkWorld(this IServiceCollection sc)
        {
            sc.AddNetworkWorldMessage();

            sc.AddSingletonLegacy<IEntityCommandManager, EntityCommandManager>();
            sc.AddSingletonLegacy<IChatFormatManager, ChatFormatManager>();
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using NexusForever.Server.Character.Network.Internal.Handler.Player;
using NexusForever.Server.Character.Network.Internal.Handler.Who;
using Rebus.Config;

namespace NexusForever.Server.Character.Network.Internal.Handler
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNetworkInternalHandlers(this IServiceCollection sc)
        {
            sc.AddRebusHandler<PlayerGuildAssociationUpdatedHandler>();
            sc.AddRebusHandler<PlayerInfoRequestHandler>();
            sc.AddRebusHandler<PlayerLoggedInHandler>();
            sc.AddRebusHandler<PlayerLoggedOutHandler>();
            sc.AddRebusHandler<PlayerStatUpdatedHandler>();
            sc.AddRebusHandler<PlayerWorldZoneUpdatedHandler>();

            sc.AddRebusHandler<WhoRequestHandler>();
        }
    }
}

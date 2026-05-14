using Microsoft.Extensions.DependencyInjection;
using NexusForever.Server.Character.Network.Internal.Handler.Player;
using Rebus.Config;

namespace NexusForever.Server.Character.Network.Internal.Handler
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNetworkInternalHandlers(this IServiceCollection sc)
        {
            sc.AddRebusHandler<PlayerInfoRequestHandler>();
        }
    }
}

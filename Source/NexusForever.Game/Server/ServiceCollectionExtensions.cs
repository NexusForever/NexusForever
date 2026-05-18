using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Server;
using NexusForever.Shared;

namespace NexusForever.Game.Server
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameServer(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<IServerManager, ServerManager>();
        }
    }
}

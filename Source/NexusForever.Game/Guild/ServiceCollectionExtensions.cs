using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Shared;

namespace NexusForever.Game.Guild
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameGuild(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<IGlobalGuildManager, GlobalGuildManager>();
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Reputation;
using NexusForever.Shared;

namespace NexusForever.Game.Reputation
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameReputation(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<IFactionManager, FactionManager>();
        }
    }
}

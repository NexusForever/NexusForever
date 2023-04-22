using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Shared;

namespace NexusForever.Game.Cinematic
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameCinematic(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<IGlobalCinematicManager, GlobalCinematicManager>();
        }
    }
}

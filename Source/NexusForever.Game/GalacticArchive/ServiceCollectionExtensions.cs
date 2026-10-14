using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.GalacticArchive;
using NexusForever.Shared;

namespace NexusForever.Game.GalacticArchive
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameGalacticArchive(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<IGlobalGalacticArchiveManager, GlobalGalacticArchiveManager>();
        }
    }
}

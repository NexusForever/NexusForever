using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Shared;

namespace NexusForever.Game.Housing
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameHousing(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<IGlobalResidenceManager, GlobalResidenceManager>();
        }
    }
}

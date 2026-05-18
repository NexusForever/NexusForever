using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Achievement;
using NexusForever.Shared;

namespace NexusForever.Game.Achievement
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameAchievement(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<IGlobalAchievementManager, GlobalAchievementManager>();
        }
    }
}

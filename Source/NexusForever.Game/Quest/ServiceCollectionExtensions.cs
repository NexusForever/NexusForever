using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Shared;

namespace NexusForever.Game.Quest
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameQuest(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<IGlobalQuestManager, GlobalQuestManager>();
        }
    }
}

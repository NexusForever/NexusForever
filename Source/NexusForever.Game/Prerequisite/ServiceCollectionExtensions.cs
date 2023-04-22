using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Shared;

namespace NexusForever.Game.Prerequisite
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGamePrerequisite(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<IPrerequisiteManager, PrerequisiteManager>();
        }
    }
}

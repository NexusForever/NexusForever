using System.Reflection;
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

            sc.AddTransientFactory<IPrerequisiteParameters, PrerequisiteParameters>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                PrerequisiteCheckAttribute attribute = type.GetCustomAttribute<PrerequisiteCheckAttribute>();
                if (attribute == null)
                    continue;

                sc.AddKeyedTransient(typeof(IPrerequisiteCheck), attribute.Type, type);
            }
        }
    }
}

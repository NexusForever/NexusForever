using System.Reflection;
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
            sc.AddTransient<ICinematicFactory, CinematicFactory>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.IsAssignableTo(typeof(ICinematicBase)) || type == typeof(CinematicBase))
                    continue;

                Type interfaceType = type.GetInterface($"I{type.Name}");
                if (interfaceType == null)
                    continue;

                sc.AddTransient(interfaceType, type);
            }
        }
    }
}

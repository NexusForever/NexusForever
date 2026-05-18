using System;
using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Shared
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSingletonLegacy<TInterface, TImplementation>(this IServiceCollection sc)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            sc.AddSingleton<TImplementation>();
            sc.AddSingleton<TInterface>(sp => sp.GetService<TImplementation>());
        }

        public static void AddTransientFactory<TInterface, TImplementation>(this IServiceCollection sc)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            sc.AddTransient<TInterface, TImplementation>();
            sc.AddSingleton<IFactory<TInterface>, Factory<TInterface>>();
        }

        public static void AddShared(this IServiceCollection sc)
        {
            sc.AddSingleton<IWorldManager, WorldManager>();
        }
    }
}

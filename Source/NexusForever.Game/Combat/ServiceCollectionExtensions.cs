using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract;
using NexusForever.Shared;

namespace NexusForever.Game.Combat
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameCombat(this IServiceCollection sc)
        {
            sc.AddTransientFactory<IDamageCalculator, DamageCalculator>();
        }
    }
}

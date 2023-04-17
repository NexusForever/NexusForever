using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Shared;

namespace NexusForever.Game.Spell
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameSpell(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<IGlobalSpellManager, GlobalSpellManager>();
        }
    }
}

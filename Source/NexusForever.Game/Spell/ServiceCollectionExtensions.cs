using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Proc;
using NexusForever.Game.Spell.Effect;
using NexusForever.Game.Spell.Info;
using NexusForever.Game.Spell.Proc;
using NexusForever.Game.Spell.Target;
using NexusForever.Game.Spell.Type;
using NexusForever.Game.Spell.Validator;
using NexusForever.Game.Static.Spell;
using NexusForever.Shared;

namespace NexusForever.Game.Spell
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameSpell(this IServiceCollection sc)
        {
            sc.AddGameSpellEffect();
            sc.AddGameSpellInfo();
            sc.AddGameSpellTarget();
            sc.AddGameSpellValidator();

            sc.AddSingletonLegacy<IGlobalSpellManager, GlobalSpellManager>();
            sc.AddSingletonLegacy<ISpellLookupManager, SpellLookupManager>();

            sc.AddTransientFactory<ISpellParameters, SpellParameters>();

            sc.AddTransient<ISpellFactory, SpellFactory>();
            sc.AddKeyedTransient<ISpell, SpellNormal>(CastMethod.Normal);
            sc.AddKeyedTransient<ISpell, SpellChanneled>(CastMethod.Channeled);
            sc.AddKeyedTransient<ISpell, SpellChanneledField>(CastMethod.ChanneledField);
            sc.AddKeyedTransient<ISpell, SpellClientSideInteraction>(CastMethod.ClientSideInteraction);
            sc.AddKeyedTransient<ISpell, SpellRapidTap>(CastMethod.RapidTap);
            sc.AddKeyedTransient<ISpell, SpellChargeRelease>(CastMethod.ChargeRelease);
            sc.AddKeyedTransient<ISpell, SpellMultiphase>(CastMethod.Multiphase);
            sc.AddKeyedTransient<ISpell, SpellTransactional>(CastMethod.Transactional);
            sc.AddKeyedTransient<ISpell, SpellAura>(CastMethod.Aura);

            sc.AddTransient<IProcManager,  ProcManager>();
            sc.AddTransientFactory<IProcInfo, ProcInfo>();
            sc.AddTransientFactory<IProcParameters, ProcParameters>();
        }
    }
}

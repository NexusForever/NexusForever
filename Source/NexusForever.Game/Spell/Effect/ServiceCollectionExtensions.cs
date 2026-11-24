using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Spell.Effect;
using NexusForever.Game.Abstract.Spell.Effect.Data;
using NexusForever.Game.Spell.Effect.Data;

namespace NexusForever.Game.Spell.Effect
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameSpellEffect(this IServiceCollection sc)
        {
            sc.AddSingleton<IGlobalSpellEffectManager, GlobalSpellEffectManager>();

            sc.AddTransient<ISpellEffectCCStateBreakData, SpellEffectCCStateBreakData>();
            sc.AddTransient<ISpellEffectCCStateSetData, SpellEffectCCStateSetData>();
            sc.AddTransient<ISpellEffectDamageData, SpellEffectDamageData>();
            sc.AddTransient<ISpellEffectDefaultData, SpellEffectDefaultData>();
            sc.AddTransient<ISpellEffectDisguiseData, SpellEffectDisguiseData>();
            sc.AddTransient<ISpellEffectFactionSetData, SpellEffectFactionSetData>();
            sc.AddTransient<ISpellEffectForcedMoveData, SpellEffectForcedMoveData>();
            sc.AddTransient<ISpellEffectGiveAugmentPowerToPlayerData, SpellEffectGiveAugmentPowerToPlayerData>();
            sc.AddTransient<ISpellEffectLearnDyeColourData, SpellEffectLearnDyeColourData>();
            sc.AddTransient<ISpellEffectModifyInterruptArmourData, SpellEffectModifyInterruptArmourData>();
            sc.AddTransient<ISpellEffectProcData, SpellEffectProcData>();
            sc.AddTransient<ISpellEffectProxyData, SpellEffectProxyData>();
            sc.AddTransient<ISpellEffectSpellForceRemoveData, SpellEffectSpellForceRemoveData>();
            sc.AddTransient<ISpellEffectSummonMountData, SpellEffectSummonMountData>();
            sc.AddTransient<ISpellEffectSummonVanityPetData, SpellEffectSummonVanityPetData>();
            sc.AddTransient<ISpellEffectTeleportData, SpellEffectTeleportData>();
            sc.AddTransient<ISpellEffectUnitPropertyModifierData, SpellEffectUnitPropertyModifierData>();
            sc.AddTransient<ISpellEffectUnlockInlaidAugmentData, SpellEffectUnlockInlaidAugmentData>();
            sc.AddTransient<ISpellEffectUnlockMountData, SpellEffectUnlockMountData>();
            sc.AddTransient<ISpellEffectUnlockPetFlairData, SpellEffectUnlockPetFlairData>();
            sc.AddTransient<ISpellEffectUnlockVanityPetData, SpellEffectUnlockVanityPetData>();
            sc.AddTransient<ISpellEffectVitalModifierData, SpellEffectVitalModifierData>();

            foreach (System.Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var attribute = type.GetCustomAttribute<SpellEffectHandlerAttribute>();
                if (attribute == null)
                    continue;

                foreach (System.Type interfaceType in type.GetInterfaces()
                    .Where(i => i.IsGenericType))
                {
                    if (interfaceType.GetGenericTypeDefinition() == typeof(ISpellEffectApplyHandler<>))
                        sc.AddKeyedTransient(interfaceType, attribute.SpellEffectType, type);
                    else if (interfaceType.GetGenericTypeDefinition() == typeof(ISpellEffectRemoveHandler<>))
                        sc.AddKeyedTransient(interfaceType, attribute.SpellEffectType, type);
                }
            }
        }
    }
}

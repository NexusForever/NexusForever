using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Effect;
using NexusForever.Game.Abstract.Spell.Effect.Data;
using NexusForever.Game.Abstract.Spell.Target;
using NexusForever.Game.Static.Spell;
using NexusForever.Game.Static.Spell.Effect;

namespace NexusForever.Game.Spell.Effect.Handler
{
    [SpellEffectHandler(SpellEffectType.GiveAugmentPowerToPlayer)]
    public class SpellEffectGiveAugmentPowerToPlayerHandler : ISpellEffectApplyHandler<ISpellEffectGiveAugmentPowerToPlayerData>
    {
        /// <summary>
        /// Handle <see cref="ISpell"/> effect apply on <see cref="IUnitEntity"/> target.
        /// </summary>
        /// <remarks>
        /// The single shipped type 128 row (effect 169480 of spell 67476, <c>AMP Power Unlock</c>)
        /// carries dataBits00 = 1. The client has no dispatcher case for the type at all: the grant
        /// is entirely server side and the client learns the result from
        /// <c>ServerAmpPowerUpdate</c>, or from the same field of <c>ServerPlayerCreate</c> at login.
        ///
        /// The 10 point cap is enforced before the cast, by <c>Prerequisite</c> 28557 on the spell;
        /// the clamp inside <c>GrantBonusAmpPower</c> is defence in depth for a route that has no
        /// such gate.
        /// </remarks>
        public SpellEffectExecutionResult Apply(ISpellExecutionContext executionContext, IUnitEntity target, ISpellTargetEffectInfo info, ISpellEffectGiveAugmentPowerToPlayerData data)
        {
            if (target is not IPlayer player)
                return SpellEffectExecutionResult.Ok;

            player.SpellManager.GrantBonusAmpPower((int)data.AugmentPower);

            return SpellEffectExecutionResult.Ok;
        }
    }
}

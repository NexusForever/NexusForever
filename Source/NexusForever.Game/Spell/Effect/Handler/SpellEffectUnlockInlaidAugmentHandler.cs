using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Effect;
using NexusForever.Game.Abstract.Spell.Effect.Data;
using NexusForever.Game.Abstract.Spell.Target;
using NexusForever.Game.Static.Spell;
using NexusForever.Game.Static.Spell.Effect;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Spell.Effect.Handler
{
    [SpellEffectHandler(SpellEffectType.UnlockInlaidAugment)]
    public class SpellEffectUnlockInlaidAugmentHandler : ISpellEffectApplyHandler<ISpellEffectUnlockInlaidAugmentData>
    {
        #region Dependency Injection

        private readonly IGameTableManager gameTableManager;

        public SpellEffectUnlockInlaidAugmentHandler(
            IGameTableManager gameTableManager)
        {
            this.gameTableManager = gameTableManager;
        }

        #endregion

        /// <summary>
        /// Handle <see cref="ISpell"/> effect apply on <see cref="IUnitEntity"/> target.
        /// </summary>
        public SpellEffectExecutionResult Apply(ISpellExecutionContext executionContext, IUnitEntity target, ISpellTargetEffectInfo info, ISpellEffectUnlockInlaidAugmentData data)
        {
            if (target is not IPlayer player)
                return SpellEffectExecutionResult.Ok;

            EldanAugmentationEntry entry = gameTableManager.EldanAugmentation.GetEntry(data.EldanAugmentationId);

            // 7 of the 281 shipped unlock spells name an augmentation row that does not exist. Ok rather than
            // PreventEffect, so the effect is still part of ServerSpellGo.
            if (entry == null)
                return SpellEffectExecutionResult.Ok;

            player.SpellManager.UnlockInlaidEldanAugmentation((ushort)entry.Id);

            return SpellEffectExecutionResult.Ok;
        }
    }
}

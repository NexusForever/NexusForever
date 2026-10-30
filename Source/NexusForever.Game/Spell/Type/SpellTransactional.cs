using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Effect;
using NexusForever.Game.Abstract.Spell.Target;
using NexusForever.Game.Abstract.Spell.Validator;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Game.Spell.Type
{
    /// <summary>
    /// A cast that trades something the caster owns for what the spell grants, such as an unlock item.
    /// </summary>
    /// <remarks>
    /// The client has no code that distinguishes this cast method from <see cref="CastMethod.Normal"/>:
    /// there is no cast bar, no channel, no threshold and no client-side confirmation step, and every
    /// spell that uses it has no cast time, no cooldown and no cost. The timeline is therefore
    /// <see cref="SpellNormal"/>'s. What this cast method adds is a server-side rule: the item and the
    /// effects of the cast are one unit of work. Everything that can refuse the cast runs before
    /// anything is taken, and the item is taken where every other cost is taken - on the world thread,
    /// in the execute step, immediately before the effects run.
    /// </remarks>
    public class SpellTransactional : SpellNormal
    {
        public override CastMethod CastMethod => CastMethod.Transactional;

        #region Dependency Injection

        private readonly ILogger<SpellTransactional> log;
        private readonly IGlobalSpellEffectManager globalSpellEffectManager;

        public SpellTransactional(
            ILogger<SpellTransactional> log,
            ISpellTargetInfoCollection spellTargetInfoCollection,
            IGlobalSpellManager globalSpellManager,
            ICastResultValidatorManager castResultValidatorManager,
            IDisableManager disableManager,
            IGlobalSpellEffectManager globalSpellEffectManager)
            : base(log, spellTargetInfoCollection, globalSpellManager, castResultValidatorManager, disableManager)
        {
            this.log                      = log;
            this.globalSpellEffectManager = globalSpellEffectManager;
        }

        #endregion

        public override bool Cast()
        {
            // What this cast takes cannot be given back, so a spell whose effects this server cannot
            // execute must be refused before anything is taken rather than consume the item and do
            // nothing. This is a check on the server's own capability, not on the caster, so it runs
            // before the cooldown and prerequisite checks and never puts the caster on cooldown.
            foreach (Spell4EffectsEntry entry in Parameters.SpellInfo.Effects)
            {
                if (globalSpellEffectManager.GetSpellEffectApplyHandlerType(entry.EffectType) != null)
                    continue;

                log.LogWarning($"Refusing transactional spell {Parameters.SpellInfo.Entry.Id}: no apply handler for effect {entry.EffectType}.");

                SendSpellCastResult(CastResult.SpellBad);
                status = SpellStatus.Failed;
                return false;
            }

            return base.Cast();
        }

        /// <summary>
        /// Take the item this cast consumes, then run the normal execute step.
        /// </summary>
        protected override void Execute(bool handleCDAndCost = true)
        {
            // Mirrors the condition the base class uses for the other costs, so the item is taken once
            // per cast and at the same moment as them.
            if (handleCDAndCost && (currentPhase == 0 || currentPhase == 255) && !ConsumeItem())
                return;

            base.Execute(handleCDAndCost);
        }

        /// <summary>
        /// Consume the item this cast was requested with, returning whether the cast may continue.
        /// </summary>
        private bool ConsumeItem()
        {
            // Casts that did not come from an item use - scripts, proxies, anything server-initiated -
            // have no item to take and are unaffected.
            if (Parameters.ConsumeItemLocation == null || Caster is not IPlayer player)
                return true;

            IItem item = player.Inventory.GetItem(Parameters.ConsumeItemLocation);
            if (item == null || item.Guid != Parameters.ConsumeItemGuid)
            {
                log.LogTrace($"Transactional spell {Parameters.SpellInfo.Entry.Id} aborted: item {Parameters.ConsumeItemGuid} is no longer in the slot it was used from.");
                return Abort();
            }

            // ItemUse is this server's single-unit consume rule: one charge, or one off the stack, and
            // the item is deleted when the last one goes. It returns false when there is nothing left
            // to take, which is the "enough stack" check.
            if (!player.Inventory.ItemUse(item))
            {
                log.LogTrace($"Transactional spell {Parameters.SpellInfo.Entry.Id} aborted: item {item.Guid} had nothing left to consume.");
                return Abort();
            }

            return true;
        }

        /// <summary>
        /// End the cast without executing any effect, telling the client why.
        /// </summary>
        private bool Abort()
        {
            // Not CancelCast: that only acts while the spell counts as casting, and a cast that is
            // already in its execute step must always be ended here rather than left in the caster's
            // spell list.
            SendSpellCastResult(CastResult.ItemBad);
            Finish();
            return false;
        }
    }
}

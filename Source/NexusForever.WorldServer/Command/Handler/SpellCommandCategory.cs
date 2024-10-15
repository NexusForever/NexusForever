using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Spell;
using NexusForever.Game.Static.RBAC;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.WorldServer.Command.Context;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Spell, "A collection of commands to manage spells.", "spell")]
    public class SpellCommandCategory : CommandCategory
    {
        [Command(Permission.SpellCast, "A collection of commands to cast spells.", "cast")]
        public class CurrencyAccountCommandCategory : CommandCategory
        {
            [Command(Permission.SpellCast, "Cast a base spell for target, optionally supplying the tier.", "base")]
            [CommandTarget(typeof(IUnitEntity))]
            public void HandleSpellBaseCast(ICommandContext context,
                [Parameter("Spell base id to cast from target.")]
                uint spell4BaseId,
                [Parameter("Tier of the base spell to cast from target.")]
                byte? tier)
            {
                tier ??= 1;
                CastSpell(context, spell4BaseId, tier);
            }

            [Command(Permission.SpellCast, "Cast a spell for target", "spell")]
            [CommandTarget(typeof(IUnitEntity))]
            public void HandleSpellCast(ICommandContext context,
                [Parameter("Spell id to cast from target.")]
                uint spellId)
            {
                Spell4Entry entry = GameTableManager.Instance.Spell4.GetEntry(spellId);
                if (entry == null)
                {
                    context.SendMessage($"Invalid spell id {spellId}!");
                    return;
                }

                CastSpell(context, entry.Spell4BaseIdBaseSpell, (byte)entry.TierIndex);
            }

            private void CastSpell(ICommandContext context, uint spell4BaseId, byte? tier)
            {
                ISpellBaseInfo spellBaseInfo = GlobalSpellManager.Instance.GetSpellBaseInfo(spell4BaseId);
                if (spellBaseInfo == null)
                {
                    context.SendMessage($"Invalid spell base id {spell4BaseId}!");
                    return;
                }

                ISpellInfo spellInfo = spellBaseInfo.GetSpellInfo(tier.Value);
                if (spellInfo == null)
                {
                    context.SendMessage($"Invalid tier {tier.Value} for spell base id {spell4BaseId}!");
                    return;
                }

                context.GetTargetOrInvoker<IUnitEntity>().CastSpell(spell4BaseId, tier.Value, new SpellParameters
                {
                    UserInitiatedSpellCast = false
                });
            }
        }

        [Command(Permission.SpellAdd, "Add a base spell to character, optionally supplying the tier.", "add")]
        [CommandTarget(typeof(IPlayer))]
        public void HandleSpellAdd(ICommandContext context,
            [Parameter("Spell base id to add to character.")]
            uint spell4BaseId,
            [Parameter("Tier of the base spell to add to character.")]
            byte? tier)
        {
            tier ??= 1;

            ISpellBaseInfo spellBaseInfo = GlobalSpellManager.Instance.GetSpellBaseInfo(spell4BaseId);
            if (spellBaseInfo == null)
            {
                context.SendMessage($"Invalid spell base id {spell4BaseId}!");
                return;
            }

            ISpellInfo spellInfo = spellBaseInfo.GetSpellInfo(tier.Value);
            if (spellInfo == null)
            {
                context.SendMessage($"Invalid tier {tier.Value} for spell base id {spell4BaseId}!");
                return;
            }

            context.GetTargetOrInvoker<IPlayer>().SpellManager.AddSpell(spell4BaseId, tier.Value);
        }

        [Command(Permission.SpellResetCooldown, "Reset a single spell cooldown for character, if no spell if supplied all cooldowns will be reset", "resetcooldown")]
        [CommandTarget(typeof(IPlayer))]
        public void HandleSpellResetCooldown(ICommandContext context,
            [Parameter("Spell id to reset cooldown for character.")]
            uint? spell4Id)
        {
            IPlayer target = context.GetTargetOrInvoker<IPlayer>();
            if (spell4Id.HasValue)
                target.SpellManager.SetSpellCooldown(spell4Id.Value, 0d, true);
            else
                target.SpellManager.ResetAllSpellCooldowns();
        }
    }
}

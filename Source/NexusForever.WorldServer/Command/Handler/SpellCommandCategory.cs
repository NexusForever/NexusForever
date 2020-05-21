using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.RBAC.Static;
using NexusForever.WorldServer.Game.Spell;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Spell, "A collection of commands to manage spells.")]
    public class SpellCommandCategory : CommandCategory
    {
        [Command(Permission.SpellAdd, "Add a base spell to character, optionally supplying the tier.", "add")]
        [CommandTarget(typeof(Player))]
        public void HandleSpellAdd(ICommandContext context,
            [Parameter("Spell base id to add to character.")]
            uint spell4BaseId,
            [Parameter("Tier of the base spell to add to character.")]
            byte? tier)
        {
            tier ??= 1;

            SpellBaseInfo spellBaseInfo = GlobalSpellManager.Instance.GetSpellBaseInfo(spell4BaseId);
            if (spellBaseInfo == null)
            {
                context.SendMessage($"Invalid spell base id {spell4BaseId}!");
                return;
            }

            SpellInfo spellInfo = spellBaseInfo.GetSpellInfo(tier.Value);
            if (spellInfo == null)
            {
                context.SendMessage($"Invalid tier {tier.Value} for spell base id {spell4BaseId}!");
                return;
            }

            context.GetTargetOrInvoker<Player>().SpellManager.AddSpell(spell4BaseId, tier.Value);
        }

        [Command(Permission.SpellCast, "Cast a base spell for target, optionally supplying the tier.", "cast")]
        [CommandTarget(typeof(UnitEntity))]
        public void HandleSpellCast(ICommandContext context,
            [Parameter("Spell base id to cast from target.")]
            uint spell4BaseId,
            [Parameter("Tier of the base spell to cast from target.")]
            byte? tier)
        {
            tier ??= 1;

            SpellBaseInfo spellBaseInfo = GlobalSpellManager.Instance.GetSpellBaseInfo(spell4BaseId);
            if (spellBaseInfo == null)
            {
                context.SendMessage($"Invalid spell base id {spell4BaseId}!");
                return;
            }

            SpellInfo spellInfo = spellBaseInfo.GetSpellInfo(tier.Value);
            if (spellInfo == null)
            {
                context.SendMessage($"Invalid tier {tier.Value} for spell base id {spell4BaseId}!");
                return;
            }

            context.GetTargetOrInvoker<UnitEntity>().CastSpell(spell4BaseId, tier.Value, new SpellParameters
            {
                UserInitiatedSpellCast = false
            });
        }

        [Command(Permission.SpellResetCooldown, "Reset a single spell cooldown for character, if no spell if supplied all cooldowns will be reset", "resetcooldown")]
        [CommandTarget(typeof(Player))]
        public void HandleSpellResetCooldown(ICommandContext context,
            [Parameter("Spell id to reset cooldown for character.")]
            uint? spell4Id)
        {
            Player target = context.GetTargetOrInvoker<Player>();
            if (spell4Id.HasValue)
                target.SpellManager.SetSpellCooldown(spell4Id.Value, 0d);
            else
                target.SpellManager.ResetAllSpellCooldowns();
        }
    }
}

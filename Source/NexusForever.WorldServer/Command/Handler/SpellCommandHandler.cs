using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Account.Static;
using NexusForever.WorldServer.Game.Spell;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Spell", Permission.None)]
    public class SpellCommandHandler : CommandCategory
    {
        public SpellCommandHandler()
            : base(true, "spell")
        {
        }

        [SubCommandHandler("add", "spell4BaseId - Add a spell to the character", Permission.CommandSpellAdd)]
        public Task AddSpellSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length == 0)
                return Task.CompletedTask;

            context.Session.Player.SpellManager.AddSpell(uint.Parse(parameters[0]));
            return Task.CompletedTask;
        }

        [SubCommandHandler("cast", "spell4BaseId [tier] - Cast a spell, optionally supplying the tier", Permission.CommandSpellCast)]
        public Task CastSpellSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length == 0)
                return Task.CompletedTask;

            byte tier = parameters.Length > 1 ? byte.Parse(parameters[1]) : (byte)1;
            context.Session.Player.CastSpell(uint.Parse(parameters[0]), tier, new SpellParameters
            {
                UserInitiatedSpellCast = false
            });

            return Task.CompletedTask;
        }

        [SubCommandHandler("resetcooldown", "[spell4Id] - Reset a single spell cooldown, if no spell if supplyed all cooldowns will be reset", Permission.CommandSpellResetCooldowns)]
        public Task ResetCooldownSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length > 0)
                context.Session.Player.SpellManager.SetSpellCooldown(uint.Parse(parameters[0]), 0d);
            else
                context.Session.Player.SpellManager.ResetAllSpellCooldowns();

            return Task.CompletedTask;
        }
    }
}

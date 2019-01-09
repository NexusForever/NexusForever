using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Spell")]
    public class SpellCommandHandler : CommandCategory
    {
        public SpellCommandHandler()
            : base(true, "spell")
        {
        }

        [SubCommandHandler("add", "spell4BaseId [tier] - Add a spell to the character, optionally supplying the tier")]
        public Task AddSpellSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length == 0)
                return Task.CompletedTask;

            byte tier = parameters.Length > 1 ? byte.Parse(parameters[1]) : (byte)1;
            context.Session.Player.SpellManager.AddSpell(uint.Parse(parameters[0]), tier);
            return Task.CompletedTask;
        }
    }
}

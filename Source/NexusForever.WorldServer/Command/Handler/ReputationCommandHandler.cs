using System.Linq;
using System.Threading.Tasks;
using NexusForever.Shared.GameTable;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Reputation")]
    public class ReputationCommandHandler : CommandCategory
    {
        public ReputationCommandHandler()
            : base(true, "reputation")
        {
        }

        [SubCommandHandler("add", "factionId value - Add value to the faction")]
        public Task AddReputationSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length <= 1)
                return Task.CompletedTask;

            uint factionId = uint.Parse(parameters[0]);
            uint factionValue = uint.Parse(parameters[1]);

            context.Session.Player.ReputationManager.AddValue(factionId, factionValue);
            return Task.CompletedTask;
        }
    }
}

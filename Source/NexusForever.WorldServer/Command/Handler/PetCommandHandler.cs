using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Pet")]
    public class PetCommandHandler : CommandCategory
    {
        public PetCommandHandler()
            : base(true, "pet")
        {
        }

        [SubCommandHandler("unlockflair", "petFlairId - Unlock a pet flair")]
        public Task AddFlairSubCommandHandler(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length != 1)
                return Task.CompletedTask;

            context.Session.Player.PetCustomisationManager.UnlockFlair(ushort.Parse(parameters[0]));
            return Task.CompletedTask;
        }
    }
}

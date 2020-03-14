using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Save")]
    public class SaveCommandHandler : NamedCommand
    {
        public SaveCommandHandler()
            : base(false, "save")
        {
        }

        protected override Task HandleCommandAsync(CommandContext context, string command, string[] parameters)
        {
            context.Session.Player.Save();
            return Task.CompletedTask;
        }
    }
}

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Location")]
    public class LocationCommand : NamedCommand
    {

        public override string HelpText => "Writes the current position of the player as {MapId} {X} {Y} {Z}";

        public LocationCommand(ILogger<LocationCommand> logger)
            : base(new[] {"location", "loc"}, true, logger)
        {
        }

        protected override async Task HandleCommandAsync(CommandContext context, string command, string[] parameters)
        {
            await context.SendMessageAsync(Logger,
                $"{context.Session.Player.Map.Entry.Id} {context.Session.Player.Position.X} {context.Session.Player.Position.Y} {context.Session.Player.Position.Z}");
        }
    }
}

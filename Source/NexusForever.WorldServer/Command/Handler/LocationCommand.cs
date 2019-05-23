using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Account.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Location", Permission.CommandLocation)]
    public class LocationCommand : NamedCommand
    {

        public override string HelpText => "Writes the current position of the player as {MapId} {X} {Y} {Z}";

        public LocationCommand()
            : base(true, "location", "loc")
        {
        }

        protected override async Task HandleCommandAsync(CommandContext context, string command, string[] parameters)
        {
            await context.SendMessageAsync(
                $"{context.Session.Player.Map.Entry.Id} {context.Session.Player.Position.X} {context.Session.Player.Position.Y} {context.Session.Player.Position.Z}");
        }
    }
}

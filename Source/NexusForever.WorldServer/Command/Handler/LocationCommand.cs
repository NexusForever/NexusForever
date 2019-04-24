using System.Collections.Generic;
using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Location")]
    public class LocationCommand : NamedCommand
    {

        public override string HelpText => "Writes the current position of the player as {MapId} {X} {Y} {Z}";

        public LocationCommand()
            : base(true, "location", "loc")
        {
        }

        protected override async Task HandleCommandAsync(CommandContext context, string command, string[] parameters, IEnumerable<ChatFormat> chatLinks)
        {
            await context.SendMessageAsync(
                $"{context.Session.Player.Map.Entry.Id} {context.Session.Player.Position.X} {context.Session.Player.Position.Y} {context.Session.Player.Position.Z}");
        }
    }
}

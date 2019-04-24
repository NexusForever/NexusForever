using System.Collections.Generic;
using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Teleport")]
    public class TeleportHandler : NamedCommand
    {
        public TeleportHandler()
            : base(true, "teleport", "port")
        {
        }

        protected override Task HandleCommandAsync(CommandContext context, string command, string[] parameters, IEnumerable<ChatFormat> chatLinks)
        {
            WorldSession session = context.Session;
            if (parameters.Length == 4)
                session.Player.TeleportTo(ushort.Parse(parameters[0]), float.Parse(parameters[1]),
                    float.Parse(parameters[2]), float.Parse(parameters[3]));
            else if (parameters.Length == 3)
                session.Player.TeleportTo((ushort) session.Player.Map.Entry.Id, float.Parse(parameters[0]),
                    float.Parse(parameters[1]), float.Parse(parameters[2]));

            return Task.CompletedTask;
        }
    }
}

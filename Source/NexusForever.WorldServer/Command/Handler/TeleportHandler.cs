using Microsoft.Extensions.Logging;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Teleport")]
    public class TeleportHandler : NamedCommand
    {
        public TeleportHandler(ILogger<TeleportHandler> logger)
            : base(new[] { "teleport", "port" }, true, logger)
        {
        }

        protected override void HandleCommand(CommandContext context, string command, string[] parameters)
        {
            WorldSession session = context.Session;
            if (parameters.Length == 4)
                session.Player.TeleportTo(ushort.Parse(parameters[0]), float.Parse(parameters[1]), float.Parse(parameters[2]), float.Parse(parameters[3]));
            else if (parameters.Length == 3)
            {
                //Just grab their current map and use the new coords.
                session.Player.TeleportTo((ushort)session.Player.Map.Entry.Id, float.Parse(parameters[0]), float.Parse(parameters[1]), float.Parse(parameters[2]));
            }
        }
    }
}

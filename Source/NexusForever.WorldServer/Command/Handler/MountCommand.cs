using System.Numerics;
using Microsoft.Extensions.Logging;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command.Handler
{
    public class MountCommand : NamedCommand
    {
        
        public MountCommand(ILogger<MountCommand> logger) : base("mount", true, logger)
        {

        }

        protected override void HandleCommand(CommandContext context, string command, string[] parameters)
        {
            var session = context.Session;
            var mount = new Mount(session.Player);
            var vector = new Vector3(session.Player.Position.X, session.Player.Position.Y, session.Player.Position.Z);
            session.Player.Map.EnqueueAdd(mount, vector);
        }
    }
}
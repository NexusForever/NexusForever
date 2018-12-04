using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Mounts")]
    public class MountCommandHandler : NamedCommand
    {
        public MountCommandHandler(ILogger<MountCommandHandler> logger)
            : base("mount", true, logger)
        {
        }

        protected override Task HandleCommandAsync(CommandContext context, string command, string[] parameters)
        {
            WorldSession session = context.Session;
            var mount = new Mount(session.Player);
            var vector = new Vector3(session.Player.Position.X, session.Player.Position.Y, session.Player.Position.Z);
            session.Player.Map.EnqueueAdd(mount, vector);
            return Task.CompletedTask;
        }
    }
}

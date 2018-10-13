using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network;
using System.Numerics;

namespace NexusForever.WorldServer.Command.Handler {
    public static class MiscHandler {
        [CommandHandler("teleport")]
        public static void HandleTeleport(WorldSession session, string[] parameters) {
            if (parameters.Length == 4) {
                session.Player.TeleportTo(ushort.Parse(parameters[0]), float.Parse(parameters[1]), float.Parse(parameters[2]), float.Parse(parameters[3]));
            } else if (parameters.Length == 3) {
                session.Player.TeleportTo((ushort)session.Player.Map.Entry.Id, float.Parse(parameters[0]), float.Parse(parameters[1]), float.Parse(parameters[2]));
            }
        }

        [CommandHandler("mount")]
        public static void HandleMount(WorldSession session, string[] parameters) {
            var mount = new Mount(session.Player);
            var vector = new Vector3(session.Player.Position.X, session.Player.Position.Y, session.Player.Position.Z);
            session.Player.Map.EnqueueAdd(mount, vector);
        }
    }
}

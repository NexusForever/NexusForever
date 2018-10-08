using System.Numerics;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command.Handler
{
    public static class MiscHandler
    {
        [CommandHandler("teleport")]
        public static void HandleTeleport(WorldSession session, string[] parameters)
        {
            session.Player.TeleportTo(ushort.Parse(parameters[0]), float.Parse(parameters[1]), float.Parse(parameters[2]), float.Parse(parameters[3]));
        }

        [CommandHandler("mounttest")]
        public static void HandleTest(WorldSession session, string[] parameters)
        {
            var mount = new Mount(session.Player);
            var vector = new Vector3(session.Player.Position.X, session.Player.Position.Y, session.Player.Position.Z);
            session.Player.Map.EnqueueAdd(mount, vector);
        }
    }
}

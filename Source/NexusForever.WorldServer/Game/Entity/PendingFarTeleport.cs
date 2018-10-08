using System.Numerics;

namespace NexusForever.WorldServer.Game.Entity
{
    public class PendingFarTeleport
    {
        public ushort WorldId { get; }
        public Vector3 Vector { get; }

        public PendingFarTeleport(ushort worldId, float x, float y, float z)
        {
            WorldId = worldId;
            Vector  = new Vector3(x, y, z);
        }
    }
}

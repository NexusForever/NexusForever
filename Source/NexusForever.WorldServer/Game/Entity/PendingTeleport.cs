using System.Numerics;
using NexusForever.WorldServer.Game.Map;

namespace NexusForever.WorldServer.Game.Entity
{
    public class PendingTeleport
    {
        public MapInfo Info { get; }
        public Vector3 Vector { get; }

        public PendingTeleport(MapInfo info, Vector3 vector)
        {
            Info   = info;
            Vector = vector;
        }
    }
}

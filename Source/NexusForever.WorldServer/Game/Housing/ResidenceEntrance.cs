using System.Numerics;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;

namespace NexusForever.WorldServer.Game.Housing
{
    public class ResidenceEntrance
    {
        public WorldEntry Entry { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public ResidenceEntrance(WorldLocation2Entry entry)
        {
            Entry    = GameTableManager.Instance.World.GetEntry(entry.WorldId);
            Position = new Vector3(entry.Position0, entry.Position1, entry.Position2);
            Rotation = new Quaternion(entry.Facing0, entry.Facing1, entry.Facing2, entry.Facing3);
        }
    }
}

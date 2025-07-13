using NexusForever.Network.Message;

namespace NexusForever.Network.World.Entity.Model
{
    public class LockboxEntityModel : IEntityModel
    {
        public uint Creature2Id { get; set; }
        public List<uint> OwnerUnitIds { get; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Creature2Id, 18);
            writer.Write((ushort)OwnerUnitIds.Count, 16u);
            OwnerUnitIds.ForEach(unitId => writer.Write(unitId));
        }
    }
}

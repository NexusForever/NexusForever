namespace NexusForever.Network.World.Entity.Model
{
    public class CollectableUnitEntityModel : IEntityModel
    {
        public uint CreatureId { get; set; }
        public byte ObjectiveIndex { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18);
            writer.Write(ObjectiveIndex);
        }
    }
}

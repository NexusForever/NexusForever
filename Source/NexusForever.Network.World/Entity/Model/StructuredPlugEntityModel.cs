namespace NexusForever.Network.World.Entity.Model
{
    public class StructuredPlugEntityModel : IEntityModel
    {
        public uint Creature2Id { get; set; }
        public byte CurrentTier { get; set; }
        public ushort HousingWarplotPlugInfoId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Creature2Id, 18u);
            writer.Write(CurrentTier, 2u);
            writer.Write(HousingWarplotPlugInfoId, 14u);
        }
    }
}

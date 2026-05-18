namespace NexusForever.Network.World.Entity.Model
{
    public class ScannerUnitEntityModel : IEntityModel
    {
        public uint CreatureId { get; set; }
        public uint OwnerId { get; set; }
        public string Name { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18u);
            writer.Write(OwnerId);
            writer.WriteStringWide(Name);
        }
    }
}

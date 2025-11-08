namespace NexusForever.Network.World.Entity.Model
{
    public class PetEntityModel : IEntityModel
    {
        public uint Creature2Id { get; set; }
        public uint OwnerUnitId { get; set; }
        public ushort OwnerDisplayItemId { get; set; } // not used by client
        public string Name { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Creature2Id, 18u);
            writer.Write(OwnerUnitId);
            writer.Write(OwnerDisplayItemId, 15u);
            writer.WriteStringWide(Name);
        }
    }
}

namespace NexusForever.Network.World.Entity.Model
{
    public class PetEntityModel : IEntityModel
    {
        public uint CreatureId { get; set; }
        public uint OwnerId { get; set; }
        public ushort Unknown1 { get; set; }
        public string Name { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18u);
            writer.Write(OwnerId);
            writer.Write(Unknown1, 15u);
            writer.WriteStringWide(Name);
        }
    }
}

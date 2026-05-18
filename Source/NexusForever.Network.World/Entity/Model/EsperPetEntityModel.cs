namespace NexusForever.Network.World.Entity.Model
{
    public class EsperPetEntityModel : IEntityModel
    {
        public uint CreatureId { get; set; }
        public uint OwnerId { get; set; }
        public ushort OwnerDisplayItemId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18u);
            writer.Write(OwnerId);
            writer.Write(OwnerDisplayItemId, 15u);
        }
    }
}

namespace NexusForever.Network.World.Entity.Model
{
    public class TrapEntityModel : IEntityModel
    {
        public uint CreatureId { get; set; }
        public uint OwnerId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18u);
            writer.Write(OwnerId);
        }
    }
}

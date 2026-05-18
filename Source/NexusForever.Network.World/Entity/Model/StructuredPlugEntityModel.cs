namespace NexusForever.Network.World.Entity.Model
{
    public class StructuredPlugEntityModel : IEntityModel
    {
        public uint CreatureId { get; set; }
        public byte CurrentTier { get; set; }
        public ushort SocketId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18u);
            writer.Write(CurrentTier, 2u);
            writer.Write(SocketId, 14u);
        }
    }
}

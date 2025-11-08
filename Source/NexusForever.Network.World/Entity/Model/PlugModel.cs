namespace NexusForever.Network.World.Entity.Model
{
    public class PlugModel : IEntityModel
    {
        public ushort WorldSocketId { get; set; }
        public ushort WorldId { get; set; }
        public byte PlugFlags { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(WorldSocketId, 14u);
            writer.Write(WorldId, 15u);
            writer.Write(PlugFlags, 6u); // TODO:: More research
        }
    }
}

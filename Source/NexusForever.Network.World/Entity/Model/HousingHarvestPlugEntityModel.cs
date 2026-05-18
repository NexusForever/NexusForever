namespace NexusForever.Network.World.Entity.Model
{
    public class HousingHarvestPlugEntityModel : IEntityModel
    {
        public ushort PlugId { get; set; }
        public ushort SocketId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PlugId, 15u);
            writer.Write(SocketId, 14u);
        }
    }
}

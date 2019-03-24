using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Model
{
    public class HousingPlantEntityModel : IEntityModel
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

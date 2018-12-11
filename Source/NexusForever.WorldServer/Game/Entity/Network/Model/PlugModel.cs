using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Model
{
    public class PlugModel : IEntityModel
    {
        public ushort SocketId { get; set; }
        public ushort PlugId { get; set; }
        public byte PlugFlags { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SocketId, 14u);
            writer.Write(PlugId, 15u);
            writer.Write(PlugFlags, 6u);
        }
    }
}

using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerChangeWorld)]
    public class ServerChangeWorld : IWritable
    {
        public ushort WorldId { get; set; }
        public Position Position { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(WorldId, 15);
            Position.Write(writer);
            writer.Write(Yaw);
            writer.Write(Pitch);
        }
    }
}

using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.Server07F9)]
    public class Server07F9 : IWritable
    {
        public uint ServerUniqueId { get; set; }
        public CastResult CastResult { get; set; }
        public uint CasterId { get; set; }
        public bool CancelCast { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ServerUniqueId);
            writer.Write(CastResult, 9u);
            writer.Write(CasterId);
            writer.Write(CancelCast);
        }
    }
}

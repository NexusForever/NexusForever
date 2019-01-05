using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server07FA, MessageDirection.Server)]
    public class Server07FA : IWritable
    {
        public uint CastingId { get; set; }
        public ushort Unknown5 { get; set; }
        public uint CasterId { get; set; }
        public bool Unknown10 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
            writer.Write(Unknown5, 9);
            writer.Write(CasterId);
            writer.Write(Unknown10);
        }
    }
}


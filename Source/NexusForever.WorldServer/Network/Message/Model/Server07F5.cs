using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server07F5, MessageDirection.Server)]
    public class Server07F5 : IWritable
    {
        public uint CastingId { get; set; }
        public uint CasterId { get; set; }
        public uint Unknown8 { get; set; } // something 0x07F4 loop 1 related

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
            writer.Write(CasterId);
            writer.Write(Unknown8);
        }
    }
}

using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server07F9, MessageDirection.Server)]
    public class Server07F9 : IWritable
    {
        public uint CastingId { get; set; }
        public ushort Unknown5 { get; set; }
        public uint CasterId { get; set; }
        public bool Unknown11 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
            writer.Write(Unknown5, 9);
            writer.Write(CasterId);
            writer.Write(Unknown11);
        }
    }
}

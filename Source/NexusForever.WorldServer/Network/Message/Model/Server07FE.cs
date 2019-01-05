using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server07FE, MessageDirection.Server)]

    public class Server07FE : IWritable
    {
        public uint CastingId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
        }
    }
}

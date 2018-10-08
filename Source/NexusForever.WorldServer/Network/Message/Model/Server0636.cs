using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server0636, MessageDirection.Server)]
    public class Server0636 : IWritable
    {
        public uint Unknown0 { get; set; }
        public bool Unknown4 { get; set; }
        public uint Unknown8 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);
            writer.Write(Unknown4);
            writer.Write(Unknown8);
        }
    }
}

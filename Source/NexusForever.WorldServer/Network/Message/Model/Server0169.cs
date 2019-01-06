using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server0169, MessageDirection.Server)]
    public class Server0169 : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            writer.WriteBytes(new byte[14]);
        }
    }
}

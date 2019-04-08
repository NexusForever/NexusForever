using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{

    [Message(GameMessageOpcode.ServerPathActivateResult)]
    public class ServerPathActivateResult : IWritable
    {
        public byte Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result);
        }
    }
}

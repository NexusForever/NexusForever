using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientInnateChange)]
    public class ClientInnateChange : IReadable
    {
        public byte InnateIndex { get; set; }

        public void Read(GamePacketReader reader)
        {
            InnateIndex = reader.ReadByte(8u);
        }
    }
}

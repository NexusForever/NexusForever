using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
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

using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientChangeInnate, MessageDirection.Client)]
    public class ClientChangeInnate : IReadable
    {
        public byte InnateIndex { get; private set; }

        public void Read(GamePacketReader reader)
        {
            InnateIndex = reader.ReadByte(3u);
        }
    }
}

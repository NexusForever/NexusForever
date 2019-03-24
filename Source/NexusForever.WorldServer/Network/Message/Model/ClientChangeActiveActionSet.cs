using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientChangeActiveActionSet, MessageDirection.Client)]
    public class ClientChangeActiveActionSet : IReadable
    {
        public byte ActionSetIndex { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ActionSetIndex = reader.ReadByte(3u);
        }
    }
}

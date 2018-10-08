using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientCharacterList, MessageDirection.Client)]
    public class ClientCharacterList : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}

using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientCharacterDelete)]
    public class ClientCharacterDelete : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}

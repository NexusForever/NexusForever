using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientRealmList, MessageDirection.Client)]
    public class ClientRealmList : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}

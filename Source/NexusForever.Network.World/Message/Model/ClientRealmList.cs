using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientRealmList)]
    public class ClientRealmList : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}

using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{

    [Message(GameMessageOpcode.ClientLogout)]
    public class ClientLogout : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}

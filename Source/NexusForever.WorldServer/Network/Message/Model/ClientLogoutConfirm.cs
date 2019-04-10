using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientLogoutConfirm)]
    public class ClientLogoutConfirm : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}

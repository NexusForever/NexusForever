using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientLogoutConfirm)]
    public class ClientLogoutConfirm : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}

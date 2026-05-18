using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Called from ExitNow and ConfirmCamp lua functions
    [Message(GameMessageOpcode.ClientLogoutConfirm)]
    public class ClientLogoutConfirm : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}

using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientP2PTradingDeclineInvite)]
    public class ClientP2PTradingDeclineInvite : IReadable
    {
        // Zero byte message
        public void Read(GamePacketReader reader)
        {
        }
    }
}

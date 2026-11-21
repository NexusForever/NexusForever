using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.AccountInventory
{
    [Message(GameMessageOpcode.ClientCreddExchangeInfoRequest)]
    public class ClientCreddExchangeInfoRequest : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // zero byte message
        }
    }
}

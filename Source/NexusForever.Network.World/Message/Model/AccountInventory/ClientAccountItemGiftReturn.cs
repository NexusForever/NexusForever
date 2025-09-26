using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.AccountInventory
{
    [Message(GameMessageOpcode.ClientAccountItemGiftReturn)]
    public class ClientAccountItemGiftReturn : IReadable
    {
        public string TransactionId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            TransactionId = reader.ReadWideString();
        }
    }
}

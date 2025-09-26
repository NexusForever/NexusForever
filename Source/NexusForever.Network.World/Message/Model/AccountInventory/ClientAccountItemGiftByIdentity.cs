using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.AccountInventory
{
    [Message(GameMessageOpcode.ClientAccountItemGiftByIdentity)]
    public class ClientAccountItemGiftByIdentity : IReadable
    {
        public string TransactionId { get; private set; }
        public Identity Recipient { get; private set; } = new();

        public void Read(GamePacketReader reader)
        {
            TransactionId = reader.ReadWideString();
            Recipient.Read(reader);
        }
    }
}

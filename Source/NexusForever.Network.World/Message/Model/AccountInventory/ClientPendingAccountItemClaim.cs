using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.AccountInventory
{
    [Message(GameMessageOpcode.ClientPendingAccountItemClaim)]
    public class ClientPendingAccountItemClaim : IReadable
    {
        public string TransactionId { get; private set; } // used a format like 89d2a447-df3c-4c3c-a135-d4eb8dfa4fd7 but could be anything

        public void Read(GamePacketReader reader)
        {
            TransactionId = reader.ReadWideString();
        }
    }
}

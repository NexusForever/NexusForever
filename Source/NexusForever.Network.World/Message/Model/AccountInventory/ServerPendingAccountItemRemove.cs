using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.AccountInventory
{
    [Message(GameMessageOpcode.ServerPendingAccountItemRemove)]
    public class ServerPendingAccountItemRemove : IWritable
    {
        public string TransactionId { get; set; }
        public bool Redeemed { get; set; } // not used by client

        public void Write(GamePacketWriter writer)
        {
            writer.WriteStringWide(TransactionId);
            writer.Write(Redeemed);
        }
    }
}

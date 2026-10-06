using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.AccountInventory
{
    [Message(GameMessageOpcode.ServerAccountItems)]
    public class ServerAccountItems : IWritable
    {
        public List<AccountInventoryItem> AccountItems { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountItems.Count);
            AccountItems.ForEach(w => w.Write(writer));
        }
    }
}

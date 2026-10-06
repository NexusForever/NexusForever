using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.AccountInventory
{
    // Only recorded use of this in sniffs is for using a character name change token.
    // Only use of this in the client if for StoreAccountItemLib::IsRedeemCREDDInProgress that looks at the array
    // of the AccountItemsInProgress items to see if a CREDD operation is underway. There are no sniffs recorded
    // showing a CREDD account item operation listed as InProgress.
    [Message(GameMessageOpcode.ServerAccountItemsInProgress)]
    public class ServerAccountItemsInProgress : IWritable
    {
        public uint AccountId { get; set; } // is the AccountId sent ServerRealmInfo (0x3DB), filled by server but not used by client
        public List<AccountInventoryItem> Items { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountId);
            writer.Write(Items.Count);
            Items.ForEach(item => item.Write(writer));
        }
    }
}

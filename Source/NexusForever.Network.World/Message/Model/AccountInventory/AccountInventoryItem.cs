using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.AccountInventory
{
    public class AccountInventoryItem : IWritable
    {
        public ulong UserInventoryId { get; set; } 
        public uint AccountItemId { get; set; }
        public byte Unused1 { get; set; } // sometimes set to 6, value not used by client
        public bool Unused2 { get; set; }
        public Identity Owner { get; set; } = new(); // some account items are bound to a specific character
                                                     // value is ent by server but not used by client

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UserInventoryId);
            writer.Write(AccountItemId);
            writer.Write(Unused1, 5u);
            writer.Write(Unused2);
            Owner.Write(writer);
        }
    }
}

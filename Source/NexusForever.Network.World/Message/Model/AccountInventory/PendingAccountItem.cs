using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.AccountInventory
{
    public class PendingAccountItem : IWritable
    {
        public ulong PendingItemGroupId { get; set; } // never seen to be used
        public uint AccountItemId { get; set; }
        public ulong Unused2 { get; set; }
        public string TransactionId { get; set; } // used a format like 89d2a447-df3c-4c3c-a135-d4eb8dfa4fd7 but could be anything
        public uint Unused3 { get; set; }
        public Identity Unused4 { get; set; } = new();
        public ulong Unused5 { get; set; }
        public byte Unknown { get; set; } // only see values 6 and 7 in sniffs, value of 6 means item is not giftable
        public ulong Unused6 { get; set; }
        public Identity Unused7 { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PendingItemGroupId);
            writer.Write(AccountItemId);
            writer.Write(Unused2);
            writer.WriteStringWide(TransactionId);
            writer.Write(Unused3);
            Unused4.Write(writer);
            writer.Write(Unused5);
            writer.Write(Unknown, 5u);
            writer.Write(Unused6);
            Unused7.Write(writer);
        }
    }
}

using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ServerItemFlags)]
    public class ServerItemFlags : IWritable
    {
        [Flags]
        public enum ItemFlags
        { 
            Soulbound      = 0x01,
            NotReturnable  = 0x08,
            NotSalvageable = 0x10,
            VendorWontBuy  = 0x20,
            AccountTradeable = 0x40,
            NotTradeable   = 0x80,
        }

        public ulong ItemGuid { get; set; }
        public ItemFlags Flags { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemGuid);
            writer.Write(Flags, 8u);
        }
    }
}

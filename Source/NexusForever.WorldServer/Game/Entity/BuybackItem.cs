using NexusForever.Shared.GameTable.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    public class BuybackItem
    {
        public Item2Entry Item2Entry { get; set;  }
        public byte CurrencyTypeId0 { get; set; }
        public byte CurrencyTypeId1 { get; set; }
        public ulong CurrencyAmount0 { get; set; }
        public ulong CurrencyAmount1 { get; set; }
        public uint BuybackItemId { get; set; }
        public uint Quantity { get; set; }
    }
}

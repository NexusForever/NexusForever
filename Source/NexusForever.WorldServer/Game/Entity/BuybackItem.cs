using NexusForever.Shared.GameTable.Model;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Game.Entity
{
    public class BuybackItem
    {
        public Item Item { get; set;  }
        public Dictionary<CurrencyTypeEntry, ulong> CurrencyAdditions { get; set; }
        public uint BuybackItemId { get; set; }
        public uint Quantity { get; set; }
    }
}

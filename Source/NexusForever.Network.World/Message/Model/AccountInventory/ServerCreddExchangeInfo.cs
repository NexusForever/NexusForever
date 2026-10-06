using NexusForever.Game.Static.AccountInventory;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.AccountInventory
{
    [Message(GameMessageOpcode.ServerCreddExchangeInfo)]
    public class ServerCreddExchangeInfo : IWritable
    {
        public class CreddListing : IWritable
        {
            public ulong ListingId { get; set; }
            public ulong Price { get; set; }
            public bool  IsBuyOrder { get; set; }
            public ulong ListTime { get; set; } // a Win32 FILETIME
            public ulong ExpirationTime { get; set; } // a Win32 FILETIME 

            public void Write(GamePacketWriter writer)
            {
                writer.Write(ListingId);
                writer.Write(Price);
                writer.Write(IsBuyOrder);
                writer.Write(ListTime);
                writer.Write(ExpirationTime);
            }
        }

        public uint BuyOrderCount { get; set; }
        public ulong[] BuyOrderPrices { get; set; } = new ulong[3]; // average price of 1, 10, 50 purchase
        public uint SellOrderCount { get; set; }
        public ulong[] SellOrderPrices { get; set; } = new ulong[3]; // average price of 1, 10, 50 purchase
        public List<CreddListing> Listings { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(BuyOrderCount);
            for (int i = 0; i < BuyOrderPrices.Length; i++)
            {
                writer.Write(BuyOrderPrices[i]);
            }

            writer.Write(SellOrderCount);

            for (int i = 0; i < SellOrderPrices.Length; i++)
            {
                writer.Write(SellOrderPrices[i]);
            }

            writer.Write(Listings.Count);
            Listings.ForEach(listing => listing.Write(writer));
        }
    }
}

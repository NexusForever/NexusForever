using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Marketplace
{
    // Fires for each item returned by MarketplaceLib.RequestCommodityInfo().
    // It contains information on all of the buy and sell orders out for the item.
    [Message(GameMessageOpcode.ServerCommodityInfoResults)]
    public class ServerCommodityInfoResults : IWritable
    {
        public uint Item2Id { get; set; }
        public uint BuyOrderCount { get; set; }
        public ulong[] BuyOrderPrices { get; set; } = new ulong[3]; // Price for each order quantity bucket (1, 10, 50 units)
        public uint SellOrderCount { get; set; }
        public ulong[] SellOrderPrices { get; set; } = new ulong[3]; // Price for each order quantity bucket (1, 10, 50 units)
        public List<CommodityOrder> Orders { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Item2Id, 18u);
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

            writer.Write(Orders.Count);
            Orders.ForEach(order => order.Write(writer));
        }
    }
}

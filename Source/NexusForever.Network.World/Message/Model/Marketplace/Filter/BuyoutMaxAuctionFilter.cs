namespace NexusForever.Network.World.Message.Model.Marketplace.Filter
{
    public class BuyoutMaxAuctionFilter : IAuctionFilter
    {
        public ulong BuyoutPrice { get; private set; }

        public void Read(GamePacketReader reader)
        {
            BuyoutPrice = reader.ReadULong();
        }
    }
}

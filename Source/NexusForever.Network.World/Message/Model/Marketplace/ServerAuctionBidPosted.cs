using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Marketplace
{
    [Message(GameMessageOpcode.ServerAuctionBidPosted)]
    public class ServerAuctionBidPosted : IWritable
    {
        public AuctionInfo Auction { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Auction.Write(writer);
        }
    }
}

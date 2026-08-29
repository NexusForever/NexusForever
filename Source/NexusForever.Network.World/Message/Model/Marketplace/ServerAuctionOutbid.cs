using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Marketplace
{
    [Message(GameMessageOpcode.ServerAuctionOutbid)]
    public class ServerAuctionOutbid : IWritable
    {
        public AuctionInfo Auction { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Auction.Write(writer);
        }
    }
}

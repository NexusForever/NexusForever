using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Marketplace
{
    [Message(GameMessageOpcode.ServerAuctionSearchResults)]
    public class ServerAuctionSearchResults : IWritable
    {
        public uint TotalResultCount { get; set; }
        public uint CurrentPage { get; set; }
        public List<AuctionInfo> Auctions { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TotalResultCount);
            writer.Write(CurrentPage);
            writer.Write(Auctions.Count);
            Auctions.ForEach(auction => auction.Write(writer));
        }
    }
}

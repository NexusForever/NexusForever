using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model.Marketplace
{
    [Message(GameMessageOpcode.ServerAuctionPostResult)]
    public class ServerAuctionPostResult : IWritable
    {
        public GenericError Result { get; set; }
        public AuctionInfo Auction { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result, 8u);
            Auction.Write(writer);
        }
    }
}

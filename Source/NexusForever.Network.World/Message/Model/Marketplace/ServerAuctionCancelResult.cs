using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model.Marketplace
{
    [Message(GameMessageOpcode.ServerAuctionCancelResult)]
    public class ServerAuctionCancelResult : IWritable
    {
        public AuctionInfo Auction { get; set; }
        public GenericError Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Auction.Write(writer);
            writer.Write(Result, 8u);
        }
    }
}

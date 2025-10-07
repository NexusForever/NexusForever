using NexusForever.Game.Static.Marketplace;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Marketplace
{
    [Message(GameMessageOpcode.ServerMarketplaceStatus)]
    public class ServerMarketplaceStatus : IWritable
    {
        public MarketplaceStatus Status { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Status, 32u);
        }
    }
}

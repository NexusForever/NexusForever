using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Marketplace
{
    // MarketplaceLib.RequestOwnedCommodityOrders()
    [Message(GameMessageOpcode.ClientRequestOwnedCommodityOrders)]
    public class ClientRequestOwnedCommodityOrders : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // zero byte message
        }
    }
}

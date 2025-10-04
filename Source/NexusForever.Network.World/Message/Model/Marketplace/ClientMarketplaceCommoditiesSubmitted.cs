using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Marketplace
{
    // Sent from ConfirmButtonType == MarketplaceCommoditiesSubmit
    [Message(GameMessageOpcode.ClientMarketplaceCommoditiesSubmitted)]
    public class ClientMarketplaceCommoditiesSubmitted : IReadable
    {
        public CommodityOrder Order { get; private set; } = new();

        public void Read(GamePacketReader reader)
        {
            Order.Read(reader);
        }
    }
}

using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Marketplace
{
    // MarketplaceLib.RequestOwnedItemAuctions()
    [Message(GameMessageOpcode.ClientRequestOwnedItemAuctions)]
    public class ClientRequestOwnedItemAuctions : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // zero byte message
        }
    }
}

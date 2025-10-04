using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Marketplace
{
    // MarketplaceLib::RequestCommodityInfo()
    [Message(GameMessageOpcode.ClientRequestCommodityInfo)]
    public class ClientRequestCommodityInfo : IReadable
    {
        public uint Item2Id { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Item2Id = reader.ReadUInt(18u);
        }
    }
}

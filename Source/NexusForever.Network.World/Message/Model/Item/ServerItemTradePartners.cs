using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ServerItemTradePartners)]
    public class ServerItemTradePartners : IWritable
    {
        public ulong ItemGuid { get; set; }
        public List<Identity> ItemTradePartners { get; } = [];
        public uint TradeTimeLeftMs { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemGuid);
            writer.Write(ItemTradePartners.Count);
            ItemTradePartners.ForEach(partner => partner.Write(writer));
            writer.Write(TradeTimeLeftMs);
        }
    }
}

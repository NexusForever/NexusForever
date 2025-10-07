using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model.Marketplace
{
    // Fires whenever the player attempts to post a buy or sell order on the commodities exchange.
    // Informs the player if the order was successfully placed or an error was thrown.
    [Message(GameMessageOpcode.ServerCommodityOrderResult)]
    public class ServerCommodityOrderResult : IWritable
    {
        public GenericError Result { get; set; }
        public CommodityOrder OrderPosted { get; set; }
        public ulong CostToPostOrder { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result, 8u);
            OrderPosted.Write(writer);
            writer.Write(CostToPostOrder);
        }
    }
}

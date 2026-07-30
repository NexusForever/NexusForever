using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ServerBuybackItemUpdated)]
    public class ServerBuybackItemUpdated : IWritable
    {
        public BuybackItem BuybackItem { get; set; }

        public void Write(GamePacketWriter writer)
        {
            BuybackItem.Write(writer);
        }
    }
}

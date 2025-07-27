using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ServerBuybackItems)]
    public class ServerBuybackItems : IWritable
    {
        public List<BuybackItem> BuybackItems { get; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(BuybackItems.Count);
            BuybackItems.ForEach(b => b.Write(writer));
        }
    }
}

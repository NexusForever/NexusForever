using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
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

using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
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

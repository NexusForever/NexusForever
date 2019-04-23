using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
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

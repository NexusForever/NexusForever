using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerBuybackItems)]
    public class ServerBuybackItems : IWritable
    {
        public List<BuybackItem> BuybackItems { get; } = new List<BuybackItem>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(BuybackItems.Count);
            BuybackItems.ForEach(b => b.Write(writer));
        }
    }
}

using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerBuybackItemRemoved, MessageDirection.Server)]
    public class ServerBuybackItemRemoved : IWritable
    {
        public uint BuybackItemId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(BuybackItemId);
        }
    }
}

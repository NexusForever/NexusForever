using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerChannelUpdateLoot, MessageDirection.Server)]
    public class ServerCurrencyGainText : IWritable
    {
        public byte CurrencyId { get; set; }
        public long Count { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CurrencyId, 4);
            writer.Write(Count);
        }
    }
}

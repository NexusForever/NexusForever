using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCurrencyGainText, MessageDirection.Server)]
    public class ServerCurrencyGainText : IWritable
    {
        public byte CurrencyId { get; set; }
        public long Count { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write((CurrencyId & 0b1000) != 0);
            writer.Write((CurrencyId & 0b0100) != 0);
            writer.Write((CurrencyId & 0b0010) != 0);
            writer.Write((CurrencyId & 0b0001) != 0);
            writer.Write(Count);
        }
    }
}

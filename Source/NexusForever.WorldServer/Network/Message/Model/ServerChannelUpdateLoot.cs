using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerChannelUpdateLoot)]
    public class ServerChannelUpdateLoot : IWritable
    {
        public byte CurrencyId { get; set; }
        public long Amount { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CurrencyId, 4);
            writer.Write(Amount);
        }
    }
}

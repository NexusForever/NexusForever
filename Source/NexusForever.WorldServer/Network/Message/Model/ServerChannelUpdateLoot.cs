using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerChannelUpdateLoot)]
    public class ServerChannelUpdateLoot : IWritable
    {
        public byte CurrencyId { get; set; }
        public ulong Amount { get; set; }
        public ulong SignatureBonus { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CurrencyId, 4u);
            writer.Write(Amount);
            writer.Write(SignatureBonus);
        }
    }
}

using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerChannelUpdateLoot)]
    public class ServerChannelUpdateLoot : IWritable
    {
        public CurrencyType CurrencyId { get; set; }
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

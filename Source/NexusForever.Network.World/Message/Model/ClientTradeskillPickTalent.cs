using NexusForever.Network.Message;
using NexusForever.Game.Static.Crafting;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientTradeskillPickTalent)]
    public class ClientTradeskillPickTalent : IReadable
    {
        public TradeskillType TradeskillId { get; private set; }
        public uint Tier { get; private set; }
        public uint TradeskillBonusId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            TradeskillId = reader.ReadEnum<TradeskillType>(32u);
            Tier = reader.ReadUInt();
            TradeskillBonusId = reader.ReadUInt();
        }
    }
}

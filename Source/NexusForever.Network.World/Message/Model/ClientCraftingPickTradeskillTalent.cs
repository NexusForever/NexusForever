using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCraftingPickTradeskillTalent)]
    public class ClientCraftingPickTradeskillTalent : IReadable
    {
        public TradeskillType TradeskillId { get; private set; }
        public uint Tier { get; private set; }
        public uint TradeskillBonusId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            TradeskillId = (TradeskillType)reader.ReadUInt();
            Tier = reader.ReadUInt();
            TradeskillBonusId = reader.ReadUInt();
        }
    }
}

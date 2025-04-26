using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCraftingResetTradeskillTalents)]
    public class ClientCraftingResetTradeskillTalents : IReadable
    {
        public TradeskillType TradeskillId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            TradeskillId = (TradeskillType)reader.ReadUInt();
        }
    }
}

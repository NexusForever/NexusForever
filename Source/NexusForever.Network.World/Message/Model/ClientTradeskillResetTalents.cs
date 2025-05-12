using NexusForever.Network.Message;
using NexusForever.Game.Static.Crafting;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientTradeskillResetTalents)]
    public class ClientTradeskillResetTalents : IReadable
    {
        public TradeskillType TradeskillId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            TradeskillId = reader.ReadEnum<TradeskillType>(32u);
        }
    }
}

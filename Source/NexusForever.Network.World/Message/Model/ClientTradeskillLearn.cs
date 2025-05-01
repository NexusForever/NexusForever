using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientTradeskillLearn)]
    public class ClientTradeskillLearn : IReadable
    {
        public TradeskillType ToLearnTradeskillId { get; private set; }
        public TradeskillType ToDropTradeskillId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ToLearnTradeskillId = (TradeskillType)reader.ReadUInt();
            ToDropTradeskillId = (TradeskillType)reader.ReadUInt();
        }
    }
}

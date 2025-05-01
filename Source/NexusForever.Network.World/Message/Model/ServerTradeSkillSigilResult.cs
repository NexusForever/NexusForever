using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerTradeSkillSigilResult)]
    public class ServerTradeSkillSigilResult : IWritable
    {
        public TradeskillResult TradeskillSigilResult { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TradeskillSigilResult, 32u);
        }
    }
}

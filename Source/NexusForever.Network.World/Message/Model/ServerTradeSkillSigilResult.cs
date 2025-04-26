using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerTradeSkillSigilResult)]
    public class ServerTradeSkillSigilResult : IWritable
    {
        public uint TradeskillSigilResult { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TradeskillSigilResult);
            
        }
    }
}

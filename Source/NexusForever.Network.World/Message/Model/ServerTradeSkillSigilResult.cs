using NexusForever.Network.Message;
using NexusForever.Game.Static.Crafting;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerTradeskillSigilResult)]
    public class ServerTradeskillSigilResult : IWritable
    {
        public TradeskillResult TradeskillSigilResult { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TradeskillSigilResult, 32u);
        }
    }
}

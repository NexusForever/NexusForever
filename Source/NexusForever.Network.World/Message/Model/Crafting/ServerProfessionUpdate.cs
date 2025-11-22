using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Crafting
{
    [Message(GameMessageOpcode.ServerProfessionUpdate)]
    public class ServerProfessionUpdate : IWritable
    {
        public TradeskillInfo Tradeskill { get; set; } = new TradeskillInfo();

        public void Write(GamePacketWriter writer)
        {
            Tradeskill.Write(writer);
        }
    }
}

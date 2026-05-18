using NexusForever.Game.Static;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerAccountTier)]
    public class ServerAccountTier : IWritable
    {
        public AccountTier Tier { get; set; } // 5

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Tier, 5u);
        }
    }
}

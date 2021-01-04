using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Static;

namespace NexusForever.WorldServer.Network.Message.Model
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

using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientMatchingRoleCheckResponse)]
    public class ClientMatchingRoleCheckResponse : IReadable
    {
        public Game.Static.Matching.MatchType Type { get; private set; } // Uses ReadyMatchType sent in 0x05E3 
        public Role Roles { get; private set; }
        public bool Response { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Type = reader.ReadEnum<Game.Static.Matching.MatchType>(5u);
            Roles    = reader.ReadEnum<Role>();
            Response = reader.ReadBit();
        }
    }
}

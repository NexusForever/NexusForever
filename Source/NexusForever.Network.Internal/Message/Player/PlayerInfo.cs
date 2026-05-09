using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Player
{
    public class PlayerInfo
    {
        public IdentityName IdentityName { get; set; }
        public Class Class { get; set; }
        public Game.Static.Entity.Path Path { get; set; }
        public Faction Faction { get; set; }
        public byte Level { get; set; }
        public DateTime? LastOnline { get; set; }
    }
}

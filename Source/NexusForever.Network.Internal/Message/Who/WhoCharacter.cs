using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Internal.Message.Shared;
using Path = NexusForever.Game.Static.PlayerPath.Path;

namespace NexusForever.Network.Internal.Message.Who
{
    public class WhoCharacter
    {
        public Identity Identity { get; set; }
        public IdentityName IdentityName { get; set; }
        public uint Level { get; set; }
        public Race Race { get; set; }
        public Class Class { get; set; }
        public Path Path { get; set; }
        public Faction Faction { get; set; }
        public Sex Sex { get; set; }
        public ushort WorldZoneId { get; set; }
    }
}

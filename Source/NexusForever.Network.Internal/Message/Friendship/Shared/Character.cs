using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Friendship.Shared
{
    public class Character
    {
        public Identity Identity { get; set; }
        public IdentityName IdentityName { get; set; }
        public Race Race { get; set; }
        public Class Class { get; set; }
        public Game.Static.PlayerPath.Path Path { get; set; }
        public Faction Faction { get; set; }
        public byte Level { get; set; }
        public ushort WorldZoneId { get; set; }
        public uint WorldId { get; set; }
        public DateTime? LastOnline { get; set; }
    }
}

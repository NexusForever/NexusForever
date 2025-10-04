using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.API.Model.Character
{
    public class Character
    {
        public Identity Identity { get; set; }
        public IdentityName IdentityName { get; set; }
        public uint AccountId { get; set; }
        public Sex Sex { get; set; }
        public Race Race { get; set; }
        public Class Class { get; set; }
        public Path Path { get; set; }
        public Faction Faction { get; set; }
        public ushort RealmId { get; set; }
        public ushort WorldZoneId { get; set; }
        public uint WorldId { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastOnline { get; set; }
        public Position Position { get; set; }
        public List<CharacterStat> Stats { get; set; }
    }
}

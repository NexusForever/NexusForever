using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using Path = NexusForever.Game.Static.PlayerPath.Path;

namespace NexusForever.Database.Query.Model
{
    public class CharacterModel
    {
        public ulong CharacterId { get; set; }
        public ushort RealmId { get; set; }
        public string Name { get; set; }
        public string RealmName { get; set; }
        public Race Race { get; set; }
        public Class Class { get; set; }
        public Path Path { get; set; }
        public Faction Faction { get; set; }
        public Sex Sex { get; set; }
        public ushort CurrentRealmId { get; set; }
        public ushort WorldZoneId { get; set; }
        public uint Level { get; set; }
        public string GuildName { get; set; }
        public DateTime? LastOnline { get; set; }
    }
}

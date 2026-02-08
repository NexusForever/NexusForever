using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Database.Group.Model
{
    public class CharacterModel
    {
        public ulong CharacterId { get; set; }
        public ushort RealmId { get; set; }
        public string RealmName { get; set; }
        public string Name { get; set; }
        public Sex Sex { get; set; }
        public Race Race { get; set; }
        public Class Class { get; set; }
        public Path Path { get; set; }
        public Faction Faction { get; set; }
        public ushort CurrentRealm { get; set; }
        public ushort WorldZoneId { get; set; }
        public uint MapId { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public bool StatsDirty { get; set; }
        public bool RealmDirty { get; set; }

        public List<CharacterGroupModel> Groups { get; set; } = [];
        public List<CharacterStatModel> Stats { get; set; } = [];
        public List<CharacterPropertyModel> Properties { get; set; } = [];
        public GroupInviteModel Invite { get; set; }
    }
}

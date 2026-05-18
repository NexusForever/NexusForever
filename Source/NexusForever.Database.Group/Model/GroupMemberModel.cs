using NexusForever.Game.Static.Group;

namespace NexusForever.Database.Group.Model
{
    public class GroupMemberModel
    {
        public ulong GroupId { get; set; }
        public ulong CharacterId { get; set; }
        public ushort RealmId { get; set; }
        public uint Index { get; set; }
        public GroupMemberInfoFlags Flags { get; set; }
        public bool PositionDirty { get; set; }

        public GroupModel Group { get; set; }
        public CharacterModel Character { get; set; }
    }
}

namespace NexusForever.Database.Group.Model
{
    public class CharacterGroupModel
    {
        public ulong CharacterId { get; set; }
        public ushort RealmId { get; set; }
        public uint Index { get; set; }
        public ulong GroupId { get; set; }

        public CharacterModel Character { get; set; }
        public GroupMemberModel Member { get; set; }
    }
}

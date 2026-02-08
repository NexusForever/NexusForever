namespace NexusForever.Database.Group.Model
{
    public class GroupLeaderModel
    {
        public ulong GroupId { get; set; }
        public ulong CharacterId { get; set; }
        public ushort RealmId { get; set; }

        public GroupModel Group { get; set; }
        public GroupMemberModel Member { get; set; }
    }
}

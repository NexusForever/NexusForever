namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupMemberPositionUpdatedMessage
    {
        public Shared.Group Group { get; set; }
        public List<Shared.GroupMember> UpdatedMembers { get; set; }
    }
}

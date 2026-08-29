namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupMemberJoinedMessage
    {
        public Shared.Group Group { get; set; }
        public Shared.GroupMember AddedMember { get; set; }
    }
}

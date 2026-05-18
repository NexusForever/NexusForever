namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupMemberAddedMessage
    {
        public Shared.Group Group { get; set; }
        public Shared.GroupMember AddedMember { get; set; }
    }
}

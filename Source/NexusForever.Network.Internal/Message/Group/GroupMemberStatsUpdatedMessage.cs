namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupMemberStatsUpdatedMessage
    {
        public Shared.Group Group { get; set; }
        public Shared.GroupMember Member { get; set; }
    }
}

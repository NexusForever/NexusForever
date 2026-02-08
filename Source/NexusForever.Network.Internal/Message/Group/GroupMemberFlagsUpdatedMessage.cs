namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupMemberFlagsUpdatedMessage
    {
        public Shared.Group Group { get; set; }
        public Shared.GroupMember Member { get; set; }
        public bool FromPromotion { get; set; }
    }
}

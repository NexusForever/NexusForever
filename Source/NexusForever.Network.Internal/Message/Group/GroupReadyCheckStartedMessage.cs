namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupReadyCheckStartedMessage
    {
        public Shared.Group Group { get; set; }
        public Shared.GroupMember Member { get; set; }
        public string Message { get; set; }
    }
}

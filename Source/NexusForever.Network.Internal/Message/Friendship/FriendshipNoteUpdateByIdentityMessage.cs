using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipNoteUpdateByIdentityMessage
    {
        public Identity Source { get; set; }
        public Identity Target { get; set; }
        public string Note { get; set; }
    }
}

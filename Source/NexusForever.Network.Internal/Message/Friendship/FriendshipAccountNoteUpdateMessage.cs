using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipAccountNoteUpdateMessage
    {
        public Identity Identity { get; set; }
        public uint AccountId { get; set; }
        public ulong FriendAccountId { get; set; }
        public string Note { get; set; }
    }
}

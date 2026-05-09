using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipAccountNicknameUpdateMessage
    {
        public Identity Source { get; set; }
        public uint AccountId { get; set; }
        public string AccountNickname { get; set; }
    }
}

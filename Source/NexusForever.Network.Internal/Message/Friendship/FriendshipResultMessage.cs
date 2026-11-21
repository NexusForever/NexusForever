using NexusForever.Game.Static.Friendship;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipResultMessage
    {
        public Identity Target { get; set; }
        public FriendshipResult Result { get; set; }
    }
}

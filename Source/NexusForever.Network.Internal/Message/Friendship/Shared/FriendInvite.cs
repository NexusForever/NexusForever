using NexusForever.Game.Static.Friendship;

namespace NexusForever.Network.Internal.Message.Friendship.Shared
{
    public class FriendInvite
    {
        public ulong Id { get; set; }
        public Character Character { get; set; }
        public Character InviterCharacter { get; set; }
        public FriendshipType Type { get; set; }
        public bool Seen { get; set; }
        public string Note { get; set; }
        public DateTime Expiration { get; set; }
    }
}

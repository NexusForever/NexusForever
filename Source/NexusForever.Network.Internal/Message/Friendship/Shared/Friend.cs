using NexusForever.Game.Static.Friendship;

namespace NexusForever.Network.Internal.Message.Friendship.Shared
{
    public class Friend
    {
        public ulong Id { get; set; }
        public Character InviterCharacter { get; set; }
        public Character InviteeCharacter { get; set; }
        public FriendshipType Type { get; set; }
        public string Note { get; set; }
    }
}

using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.Internal.Message.Friendship.Shared
{
    public class Account
    {
        public uint Id { get; set; }
        public string Email { get; set; }
        public string Nickname { get; set; }
        public string Status { get; set; }
        public AccountPresenceState Presence { get; set; }
        public bool BlockAccountFriendRequests { get; set; }
        public Character ActiveCharacter { get; set; }
        public DateTime? LastOnline { get; set; }
    }
}

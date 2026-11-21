using NexusForever.Game.Static.Chat;

namespace NexusForever.Database.Friendship.Model
{
    public class AccountModel
    {
        public uint AccountId { get; set; }
        public string Email { get; set; }
        public string Nickname { get; set; }
        public string Status { get; set; }
        public AccountPresenceState Presence { get; set; }
        public bool BlockAccountFriendRequests { get; set; }
        public bool InvitePrivilegesSuspended { get; set; }
        public ulong? ActiveCharacterId { get; set; }
        public ushort? ActiveRealmId { get; set; }
        public DateTime? LastOnline { get; set; }

        public List<AccountFriendModel> Friends { get; set; } = [];
        public List<AccountFriendInverseModel> FriendsInverse { get; set; } = [];
        public List<AccountFriendInviteModel> FriendInvites { get; set; } = [];
        public List<AccountFriendInvitePendingModel> FriendInvitesPending { get; set; } = [];
        public CharacterModel ActiveCharacter { get; set; }
    }
}

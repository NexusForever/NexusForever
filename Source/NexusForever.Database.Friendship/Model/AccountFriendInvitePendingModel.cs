namespace NexusForever.Database.Friendship.Model
{
    public class AccountFriendInvitePendingModel
    {
        public uint AccountId { get; set; }
        public ulong FriendAccountInviteId { get; set; }

        public AccountModel Account { get; set; }
        public FriendAccountInviteModel FriendAccountInvite { get; set; }
    }
}

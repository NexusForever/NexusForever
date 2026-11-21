namespace NexusForever.Database.Friendship.Model
{
    public class AccountFriendModel
    {
        public uint AccountId { get; set; }
        public ulong FriendAccountId { get; set; }

        public AccountModel Account { get; set; }
        public FriendAccountModel FriendAccount { get; set; }
    }
}

namespace NexusForever.Database.Friendship.Model
{
    public class FriendAccountModel
    {
        public ulong Id { get; set; }
        public uint InviterAccountId { get; set; }
        public uint InviteeAccountId { get; set; }
        public string Note { get; set; }

        public AccountModel InviterAccount { get; set; }
        public AccountModel InviteeAccount { get; set; }
    }
}

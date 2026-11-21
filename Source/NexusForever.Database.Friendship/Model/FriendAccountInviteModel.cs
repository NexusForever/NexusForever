namespace NexusForever.Database.Friendship.Model
{
    public class FriendAccountInviteModel
    {
        public ulong Id { get; set; }
        public uint InviteeAccountId { get; set; }
        public uint InviterAccountId { get; set; }
        public bool Seen { get; set; }
        public string Note { get; set; }
        public DateTime Expiration { get; set; }

        public AccountModel InviteeAccount { get; set; }
        public AccountModel InviterAccount { get; set; }
    }
}

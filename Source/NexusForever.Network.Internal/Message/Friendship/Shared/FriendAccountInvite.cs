namespace NexusForever.Network.Internal.Message.Friendship.Shared
{
    public class FriendAccountInvite
    {
        public ulong Id { get; set; }
        public Account InviteeAccount { get; set; }
        public Account InviterAccount { get; set; }
        public bool Seen { get; set; }
        public string Note { get; set; }
        public DateTime Expiration { get; set; }
    }
}

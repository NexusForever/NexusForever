namespace NexusForever.Network.Internal.Message.Friendship.Shared
{
    public class FriendAccount
    {
        public ulong Id { get; set; }
        public Account InviterAccount { get; set; }
        public Account InviteeAccount { get; set; }
        public string Note { get; set; }
    }
}

namespace NexusForever.Database.Friendship.Model
{
    public class CharacterFriendInvitePendingModel
    {
        public ulong CharacterId { get; set; }
        public ushort RealmId { get; set; }
        public ulong FriendInviteId { get; set; }

        public CharacterModel Character { get; set; }
        public FriendInviteModel FriendInvite { get; set; }
    }
}

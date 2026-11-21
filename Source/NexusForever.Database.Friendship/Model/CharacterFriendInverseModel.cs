namespace NexusForever.Database.Friendship.Model
{
    public class CharacterFriendInverseModel
    {
        public ulong CharacterId { get; set; }
        public ushort RealmId { get; set; }
        public ulong FriendId { get; set; }

        public CharacterModel Character { get; set; }
        public FriendModel Friend { get; set; }
    }
}

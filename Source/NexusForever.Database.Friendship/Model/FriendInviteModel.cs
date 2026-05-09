namespace NexusForever.Database.Friendship.Model
{
    public class FriendInviteModel
    {
        public ulong Id { get; set; }
        public ulong InviteeCharacterId { get; set; }
        public ushort InviteeRealmId { get; set; }
        public ulong InviterCharacterId { get; set; }
        public ushort InviterRealmId { get; set; }
        public bool Seen { get; set; }
        public string Note { get; set; }
        public DateTime Expiration { get; set; }

        public CharacterModel InviteeCharacter { get; set; }
        public CharacterModel InviterCharacter { get; set;}
    }
}

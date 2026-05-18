namespace NexusForever.Database.Group.Model
{
    public class GroupInviteModel
    {
        public ulong GroupId { get; set; }
        public ulong InviteeCharacterId { get; set; }
        public ushort InviteeRealmId { get; set; }
        public ulong InviterCharacterId { get; set; }
        public ushort InviterRealmId { get; set; }
        public DateTime Expiration { get; set; }

        public GroupModel Group { get; set; }
        public CharacterModel InviteeCharacter { get; set; }
    }
}

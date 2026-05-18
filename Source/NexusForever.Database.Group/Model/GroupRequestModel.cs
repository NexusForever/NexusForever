using NexusForever.Game.Static.Group;

namespace NexusForever.Database.Group.Model
{
    public class GroupRequestModel
    {
        public ulong GroupId { get; set; }
        public ulong RequesterCharacterId { get; set; }
        public ushort RequesterRealmId { get; set; }
        public ulong RequesteeCharacterId { get; set; }
        public ushort RequesteeRealmId { get; set; }
        public GroupRequestType RequestType { get; set; }
        public DateTime Expiration { get; set; }

        public GroupModel Group { get; set; }
        public CharacterModel Requester { get; set; }
        public CharacterModel Requestee { get; set; }
    }
}

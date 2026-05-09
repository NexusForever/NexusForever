using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;

namespace NexusForever.Database.Friendship.Model
{
    public class CharacterModel
    {
        public ulong CharacterId { get; set; }
        public ushort RealmId { get; set; }
        public string RealmName { get; set; }
        public string Name { get; set; }
        public uint AccountId { get; set; }
        public Race Race { get; set; }
        public Class Class { get; set; }
        public Game.Static.Entity.Path Path { get; set; }
        public Faction Faction { get; set; }
        public ushort WorldZoneId { get; set; }
        public uint WorldId { get; set; }
        public DateTime? LastOnline { get; set; }

        public AccountModel Account { get; set; }
        public List<CharacterFriendModel> Friends { get; set; } = [];
        public List<CharacterFriendInverseModel> FriendsInverse { get; set; } = [];
        public List<CharacterFriendInviteModel> FriendInvites { get; set; } = [];
        public List<CharacterFriendInvitePendingModel> FriendInvitesPending { get; set; } = [];
        public List<CharacterStatModel> Stats { get; set; } = [];
    }
}

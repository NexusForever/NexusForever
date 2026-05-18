using NexusForever.Game.Static.Reputation;

namespace NexusForever.Database.Chat.Model
{
    public class CharacterModel
    {
        public ulong CharacterId { get; set; }
        public ushort RealmId { get; set; }
        public string RealmName { get; set; }
        public string Name { get; set; }
        public Faction Faction { get; set; }
        public bool IsOnline { get; set; }

        public List<CharacterChatChannelModel> Channels { get; set; } = [];
    }
}

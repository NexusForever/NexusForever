using System.Text;
using NexusForever.Game.Abstract.Chat;
using NexusForever.Game.Static.Chat;
using NexusForever.GameTable;
using NexusForever.Network.World.Chat.Model;
using NexusForever.Network.World.Message.Model.Chat;
using NetworkIdentity = NexusForever.Network.World.Message.Model.Shared.Identity;

namespace NexusForever.Game.Chat
{
    public class ChatMessageBuilder : IChatMessageBuilder
    {
        public ChatChannelType Type { get; set; }
        public ulong ChatId { get; set; }
        public bool GM { get; set; }
        public bool Self { get; set; }
        public bool AutoResponse { get; set; }
        public ulong FromCharacterId { get; set; }
        public ushort FromCharacterRealmId { get; set; }
        public string FromName { get; set; }
        public string FromRealm { get; set; }
        public AccountPresenceState PresenceState { get; set; }
        public string Text
        {
            get => builder.ToString();
            set => builder.Append(value);
        }
        public List<ChatFormat> Formats { get; set; } = new();
        public bool CrossFaction { get; set; }
        public uint Guid { get; set; }
        public byte PremiumTier { get; set; }

        private readonly StringBuilder builder = new();

        /// <summary>
        /// Append text to the end of the message.
        /// </summary>
        public void AppendText(string text)
        {
            builder.Append(text);
        }

        /// <summary>
        /// Append an item chat link to the end of the message.
        /// </summary>
        public void AppendItem(uint itemId)
        {
            if (GameTableManager.Instance.Item.GetEntry(itemId) == null)
                throw new ArgumentException($"Invalid item entry id {itemId}!");

            builder.Append("[I]");
            Formats.Add(new ChatFormat
            {
                Type        = ChatFormatType.ItemId,
                StartIndex  = (ushort)(builder.Length - 3),
                StopIndex   = (ushort)builder.Length,
                Model       = new ChatFormatItemId
                {
                    Item2Id = itemId
                }
            });
        }

        /// <summary>
        /// Append a quest chat link to the end of the message.
        /// </summary>
        public void AppendQuest(ushort questId)
        {
            if (GameTableManager.Instance.Quest2.GetEntry(questId) == null)
                throw new ArgumentException($"Invalid quest entry id {questId}!");

            builder.Append("[Q]");
            Formats.Add(new ChatFormat
            {
                Type        = ChatFormatType.QuestId,
                StartIndex  = (ushort)(builder.Length - 3),
                StopIndex   = (ushort)builder.Length,
                Model       = new ChatFormatQuestId
                {
                    Quest2Id = questId
                }
            });
        }

        /// <summary>
        /// Build message and <see cref="ChatFormat"/>'s into a <see cref="ServerChat"/>.
        /// </summary>
        public ServerChat Build()
        {
            return new ServerChat
            {
                Channel       = new Channel
                {
                    ChatChannelId = Type,
                    ChatId = ChatId
                },
                GM            = GM,
                Self          = Self,
                AutoResponse  = AutoResponse,
                From          = new NetworkIdentity
                {
                    RealmId     = FromCharacterRealmId,
                    Id          = FromCharacterId
                },
                FromName      = FromName,
                FromRealm     = FromRealm,
                PresenceState = PresenceState,
                Text          = builder.ToString(),
                Formats       = Formats,
                CrossFaction  = CrossFaction,
                UnitId          = Guid,
                PremiumTier   = PremiumTier
            };
        }
    }
}

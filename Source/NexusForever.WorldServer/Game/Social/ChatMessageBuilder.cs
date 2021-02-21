using System;
using System.Collections.Generic;
using System.Text;
using NexusForever.Shared.GameTable;
using NexusForever.WorldServer.Game.Social.Model;
using NexusForever.WorldServer.Game.Social.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Game.Social
{
    public class ChatMessageBuilder
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
        public ChatPresenceState PresenceState { get; set; }
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
                Type        = ChatFormatType.ItemItemId,
                StartIndex  = (ushort)(builder.Length - 3),
                StopIndex   = (ushort)builder.Length,
                FormatModel = new ChatFormatItemId
                {
                    ItemId = itemId
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
                FormatModel = new ChatFormatQuestId
                {
                    QuestId = questId
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
                    Type   = Type,
                    ChatId = ChatId
                },
                GM            = GM,
                Self          = Self,
                AutoResponse  = AutoResponse,

                From          = new TargetPlayerIdentity
                {
                    RealmId     = FromCharacterRealmId,
                    CharacterId = FromCharacterId
                },
               
                FromName      = FromName,
                FromRealm     = FromRealm,
                PresenceState = PresenceState,
                Text          = builder.ToString(),
                Formats       = Formats,
                CrossFaction  = CrossFaction,
                Guid          = Guid,
                PremiumTier   = PremiumTier
            };
        }
    }
}

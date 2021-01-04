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
        private readonly ChatChannelType channel;

        private readonly StringBuilder builder = new StringBuilder();
        private readonly List<ChatFormat> formats = new List<ChatFormat>();

        /// <summary>
        /// Create a new <see cref="ChatMessageBuilder"/> with the supplied <see cref="ChatChannelType"/>.
        /// </summary>
        public ChatMessageBuilder(ChatChannelType channel)
        {
            this.channel = channel;
        }

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
            formats.Add(new ChatFormat
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
            formats.Add(new ChatFormat
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
                Channel = channel,
                Text    = builder.ToString(),
                Formats = new List<ChatFormat>(formats)
            };
        }
    }
}

using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.Internal.Message.Chat.Shared.Format.Model
{
    public class ChatChannelTextQuestIdFormat : IChatChannelTextFormatModel
    {
        public ChatFormatType Type => ChatFormatType.QuestId;
        public ushort QuestId { get; set; }
    }
}

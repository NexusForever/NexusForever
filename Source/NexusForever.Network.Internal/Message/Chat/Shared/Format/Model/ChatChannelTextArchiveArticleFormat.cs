using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.Internal.Message.Chat.Shared.Format.Model
{
    public class ChatChannelTextArchiveArticleFormat : IChatChannelTextFormatModel
    {
        public ChatFormatType Type => ChatFormatType.ArchiveArticle;
        public ushort ArchiveArticleId { get; set; }
    }
}

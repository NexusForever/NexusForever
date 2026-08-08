using NexusForever.Game.Abstract.Chat.Format;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Chat.Shared.Format;
using NexusForever.Network.Internal.Message.Chat.Shared.Format.Model;
using NexusForever.Network.World.Chat;
using NexusForever.Network.World.Chat.Model;

namespace NexusForever.Game.Chat.Format.Formatter
{
    public class ArchiveArticleChatFormatter : IInternalChatFormatter<ChatFormatArchiveArticle>, INetworkChatFormatter<ChatChannelTextArchiveArticleFormat>
    {
        public IChatChannelTextFormatModel ToInternal(IPlayer player, ChatFormatArchiveArticle format)
        {
            return new ChatChannelTextArchiveArticleFormat
            {
                ArchiveArticleId = format.ArchiveArticleId,
            };
        }

        public IChatFormatModel ToNetwork(ChatChannelTextArchiveArticleFormat format)
        {
            return new ChatFormatArchiveArticle
            {
                ArchiveArticleId = format.ArchiveArticleId,
            };
        }
    }
}

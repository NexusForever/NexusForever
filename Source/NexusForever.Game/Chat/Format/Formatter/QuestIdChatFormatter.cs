using NexusForever.Game.Abstract.Chat.Format;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Chat.Shared.Format;
using NexusForever.Network.Internal.Message.Chat.Shared.Format.Model;
using NexusForever.Network.World.Chat;
using NexusForever.Network.World.Chat.Model;

namespace NexusForever.Game.Chat.Format.Formatter
{
    public class QuestIdChatFormatter : IInternalChatFormatter<ChatFormatQuestId>, INetworkChatFormatter<ChatChannelTextQuestIdFormat>
    {
        public IChatChannelTextFormatModel ToInternal(IPlayer player, ChatFormatQuestId format)
        {
            return new ChatChannelTextQuestIdFormat
            {
                QuestId = format.QuestId,
            };
        }

        public IChatFormatModel ToNetwork(ChatChannelTextQuestIdFormat format)
        {
            return new ChatFormatQuestId
            {
                QuestId = format.QuestId,
            };
        }
    }
}

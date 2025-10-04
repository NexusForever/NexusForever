using NexusForever.Game.Abstract.Chat.Format;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Chat.Shared.Format;
using NexusForever.Network.Internal.Message.Chat.Shared.Format.Model;
using NexusForever.Network.World.Chat;
using NexusForever.Network.World.Chat.Model;

namespace NexusForever.Game.Chat.Format.Formatter
{
    public class ItemIdChatFormatter : IInternalChatFormatter<ChatFormatItemId>, INetworkChatFormatter<ChatChannelTextItemIdFormat>
    {
        public IChatChannelTextFormatModel ToInternal(IPlayer player, ChatFormatItemId format)
        {
            return new ChatChannelTextItemIdFormat
            {
                ItemId = format.ItemId,
            };
        }

        IChatFormatModel INetworkChatFormatter<ChatChannelTextItemIdFormat>.ToNetwork(ChatChannelTextItemIdFormat format)
        {
            return new ChatFormatItemId
            {
                ItemId = format.ItemId,
            };
        }
    }
}

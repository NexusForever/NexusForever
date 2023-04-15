using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Social;
using NexusForever.Game.Static.Social;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Network.World.Social.Model;

namespace NexusForever.Game.Social
{
    public class ChatFormatter : IChatFormatter
    {
        /// <summary>
        /// Parse a collection of <see cref="ChatFormat"/>'s from <see cref="IPlayer"/>.
        /// </summary>
        public IEnumerable<ChatFormat> ParseChatLinks(IPlayer player, IEnumerable<ChatFormat> formats)
        {
            return formats.Select(f => ParseChatFormat(player, f));
        }

        /// <summary>
        /// Parse a <see cref="ChatFormat"/>.
        /// </summary>
        public ChatFormat ParseChatFormat(IPlayer player, ChatFormat format)
        {
            switch (format.FormatModel)
            {
                case ChatFormatItemGuid chatFormatItemGuid:
                {
                    IItem item = player.Inventory.GetItem(chatFormatItemGuid.Guid);
                    return GetChatFormatForItem(format, item);
                }
                default:
                    return format;
            }
        }

        private static ChatFormat GetChatFormatForItem(ChatFormat chatFormat, IItem item)
        {
            // TODO: this probably needs to be a full item response
            return new ChatFormat
            {
                Type        = ChatFormatType.ItemItemId,
                StartIndex  = chatFormat.StartIndex,
                StopIndex   = chatFormat.StopIndex,
                FormatModel = new ChatFormatItemId
                {
                    ItemId  = item.Info.Entry.Id
                }
            };
        }
    }
}

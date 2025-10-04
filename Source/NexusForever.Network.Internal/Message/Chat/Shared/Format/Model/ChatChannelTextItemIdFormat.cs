using NexusForever.Game.Static.Social;

namespace NexusForever.Network.Internal.Message.Chat.Shared.Format.Model
{
    public class ChatChannelTextItemIdFormat : IChatChannelTextFormatModel
    {
        public ChatFormatType Type => ChatFormatType.ItemId;
        public uint ItemId { get; set; }
    }
}

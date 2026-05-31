using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.Internal.Message.Chat.Shared.Format.Model
{
    public class ChatChannelTextItemGuidFormat : IChatChannelTextFormatModel
    {
        public ChatFormatType Type => ChatFormatType.ItemId;
        public ulong ItemGuid { get; set; }
    }
}

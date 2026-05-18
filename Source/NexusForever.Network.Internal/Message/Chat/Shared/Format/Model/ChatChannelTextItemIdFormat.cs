using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.Internal.Message.Chat.Shared.Format.Model
{
    public class ChatChannelTextItemIdFormat : IChatChannelTextFormatModel
    {
        public ChatFormatType Type => ChatFormatType.ItemId;
        public uint Item2Id { get; set; }
    }
}

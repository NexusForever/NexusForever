using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.Internal.Message.Chat.Shared.Format.Model
{
    public class ChatChannelTextLootFormat : IChatChannelTextFormatModel
    {
        public ChatFormatType Type => ChatFormatType.Loot;
        public uint LootUnitId { get; set; }
    }
}

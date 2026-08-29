using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.Internal.Message.Chat.Shared.Format.Model
{
    public class ChatChannelTextRoleplayFormat : IChatChannelTextFormatModel
    {
        public ChatFormatType Type => ChatFormatType.Roleplay;
        public bool Unknown { get; set; }
    }
}

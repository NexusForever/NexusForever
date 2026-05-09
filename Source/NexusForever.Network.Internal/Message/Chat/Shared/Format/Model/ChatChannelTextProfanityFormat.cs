using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.Internal.Message.Chat.Shared.Format.Model
{
    public class ChatChannelTextProfanityFormat : IChatChannelTextFormatModel
    {
        public ChatFormatType Type => ChatFormatType.Profanity;
        public uint RandomTextSeed { get; set; }
    }
}

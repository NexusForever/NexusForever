using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.Internal.Message.Chat.Shared.Format.Model
{
    public class ChatChannelTextAlienFormat : IChatChannelTextFormatModel
    {
        public ChatFormatType Type => ChatFormatType.Alien;
        public uint RandomTextSeed { get; set; }
    }
}

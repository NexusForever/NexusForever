using InternalChatChannelText = NexusForever.Network.Internal.Message.Chat.Shared.ChatChannelText;
using InternalChatChannelTextFormat = NexusForever.Network.Internal.Message.Chat.Shared.Format.ChatChannelTextFormat;

namespace NexusForever.Server.ChatServer.Chat
{
    public static class ChatChannelTextMappingExtensions
    {
        public static ChatChannelText ToChat(this InternalChatChannelText text)
        {
            return new ChatChannelText
            {
                Text   = text.Text,
                Format = text.Format.Select(ToChat).ToList()
            };
        }

        public static InternalChatChannelText ToInternal(this ChatChannelText text)
        {
            return new InternalChatChannelText
            {
                Text   = text.Text,
                Format = text.Format.Select(ToInternal).ToList()
            };
        }

        public static ChatChannelTextFormat ToChat(this InternalChatChannelTextFormat format)
        {
            return new ChatChannelTextFormat
            {
                Type       = format.Type,
                StartIndex = format.StartIndex,
                StopIndex  = format.StopIndex,
                Model      = format.Model
            };
        }

        public static InternalChatChannelTextFormat ToInternal(this ChatChannelTextFormat format)
        {
            return new InternalChatChannelTextFormat
            {
                Type       = format.Type,
                StartIndex = format.StartIndex,
                StopIndex  = format.StopIndex,
                Model      = format.Model
            };
        }
    }
}

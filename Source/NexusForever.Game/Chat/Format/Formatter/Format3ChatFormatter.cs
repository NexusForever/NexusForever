using NexusForever.Game.Abstract.Chat.Format;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Chat.Shared.Format;
using NexusForever.Network.Internal.Message.Chat.Shared.Format.Model;
using NexusForever.Network.World.Chat;
using NexusForever.Network.World.Chat.Model;

namespace NexusForever.Game.Chat.Format.Formatter
{
    public class Format3ChatFormatter : IInternalChatFormatter<ChatFormat3>, INetworkChatFormatter<ChatChannelTextFormat3Format>
    {
        public IChatChannelTextFormatModel ToInternal(IPlayer player, ChatFormat3 format)
        {
            return new ChatChannelTextFormat3Format
            {
                Unknown = format.Unknown,
            };
        }

        public IChatFormatModel ToNetwork(ChatChannelTextFormat3Format format)
        {
            return new ChatFormat3
            {
                Unknown = format.Unknown,
            };
        }
    }
}

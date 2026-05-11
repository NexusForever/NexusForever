using NexusForever.Game.Abstract.Chat.Format;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Chat.Shared.Format;
using NexusForever.Network.Internal.Message.Chat.Shared.Format.Model;
using NexusForever.Network.World.Chat;
using NexusForever.Network.World.Chat.Model;

namespace NexusForever.Game.Chat.Format.Formatter
{
    public class Format0ChatFormatter : IInternalChatFormatter<ChatFormat0>, INetworkChatFormatter<ChatChannelTextFormat0Format>
    {
        public IChatChannelTextFormatModel ToInternal(IPlayer player, ChatFormat0 format)
        {
            return new ChatChannelTextFormat0Format
            {
                Unknown = format.Unknown,
            };
        }

        public IChatFormatModel ToNetwork(ChatChannelTextFormat0Format format)
        {
            return new ChatFormat0
            {
                Unknown = format.Unknown,
            };
        }
    }
}

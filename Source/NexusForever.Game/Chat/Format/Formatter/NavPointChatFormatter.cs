using NexusForever.Game.Abstract.Chat.Format;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Chat.Shared.Format;
using NexusForever.Network.Internal.Message.Chat.Shared.Format.Model;
using NexusForever.Network.World.Chat;
using NexusForever.Network.World.Chat.Model;

namespace NexusForever.Game.Chat.Format.Formatter
{
    public class NavPointChatFormatter : IInternalChatFormatter<ChatFormatNavPoint>, INetworkChatFormatter<ChatChannelTextNavPointFormat>
    {
        public IChatChannelTextFormatModel ToInternal(IPlayer player, ChatFormatNavPoint format)
        {
            return new ChatChannelTextNavPointFormat
            {
                MapZoneId = format.MapZoneId,
                X = format.X,
                Y = format.Y,
            };
        }

        public IChatFormatModel ToNetwork(ChatChannelTextNavPointFormat format)
        {
            return new ChatFormatNavPoint
            {
                MapZoneId = format.MapZoneId,
                X = format.X,
                Y = format.Y,
            };
        }
    }
}

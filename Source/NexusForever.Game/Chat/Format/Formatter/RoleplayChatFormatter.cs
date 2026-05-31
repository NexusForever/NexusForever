using NexusForever.Game.Abstract.Chat.Format;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Chat.Shared.Format;
using NexusForever.Network.Internal.Message.Chat.Shared.Format.Model;
using NexusForever.Network.World.Chat;
using NexusForever.Network.World.Chat.Model;

namespace NexusForever.Game.Chat.Format.Formatter
{
    public class RoleplayChatFormatter : IInternalChatFormatter<ChatFormatRoleplay>, INetworkChatFormatter<ChatChannelTextRoleplayFormat>
    {
        public IChatChannelTextFormatModel ToInternal(IPlayer player, ChatFormatRoleplay format)
        {
            return new ChatChannelTextRoleplayFormat
            {
                Unknown = format.Unknown,
            };
        }

        public IChatFormatModel ToNetwork(ChatChannelTextRoleplayFormat format)
        {
            return new ChatFormatRoleplay
            {
                Unknown = format.Unknown,
            };
        }
    }
}

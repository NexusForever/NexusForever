using NexusForever.Game.Abstract.Chat.Format;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Chat.Shared.Format;
using NexusForever.Network.Internal.Message.Chat.Shared.Format.Model;
using NexusForever.Network.World.Chat;
using NexusForever.Network.World.Chat.Model;

namespace NexusForever.Game.Chat.Format.Formatter
{
    public class AlienChatFormatter : IInternalChatFormatter<ChatFormatAlien>, INetworkChatFormatter<ChatChannelTextAlienFormat>
    {
        public IChatChannelTextFormatModel ToInternal(IPlayer player, ChatFormatAlien format)
        {
            return new ChatChannelTextAlienFormat
            {
                RandomTextSeed = format.RandomTextSeed,
            };
        }

        public IChatFormatModel ToNetwork(ChatChannelTextAlienFormat format)
        {
            return new ChatFormatAlien
            {
                RandomTextSeed = format.RandomTextSeed,
            };
        }
    }
}

using NexusForever.Game.Abstract.Chat.Format;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Chat.Shared.Format;
using NexusForever.Network.Internal.Message.Chat.Shared.Format.Model;
using NexusForever.Network.World.Chat;
using NexusForever.Network.World.Chat.Model;

namespace NexusForever.Game.Chat.Format.Formatter
{
    public class ProfanityChatFormatter : IInternalChatFormatter<ChatFormatProfanity>, INetworkChatFormatter<ChatChannelTextProfanityFormat>
    {
        public IChatChannelTextFormatModel ToInternal(IPlayer player, ChatFormatProfanity format)
        {
            return new ChatChannelTextProfanityFormat
            {
                RandomTextSeed = format.RandomTextSeed,
            };
        }

        public IChatFormatModel ToNetwork(ChatChannelTextProfanityFormat format)
        {
            return new ChatFormatProfanity
            {
                RandomTextSeed = format.RandomTextSeed,
            };
        }
    }
}

using NexusForever.Network.Internal.Message.Chat.Shared.Format;
using NexusForever.Network.World.Chat;

namespace NexusForever.Game.Abstract.Chat.Format
{
    public interface INetworkChatFormatter<in T> where T : IChatChannelTextFormatModel
    {
        IChatFormatModel ToNetwork(T format);
    }
}

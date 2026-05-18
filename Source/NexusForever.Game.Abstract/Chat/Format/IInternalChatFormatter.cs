using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Chat.Shared.Format;
using NexusForever.Network.World.Chat;

namespace NexusForever.Game.Abstract.Chat.Format
{
    public interface IInternalChatFormatter<in T> where T : IChatFormatModel
    {
        IChatChannelTextFormatModel ToInternal(IPlayer player, T format);
    }
}

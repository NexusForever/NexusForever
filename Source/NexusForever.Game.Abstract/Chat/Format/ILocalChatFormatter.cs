using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.World.Chat;

namespace NexusForever.Game.Abstract.Chat.Format
{
    public interface ILocalChatFormatter<in T> where T : IChatFormatModel
    {
        IChatFormatModel ToLocal(IPlayer player, T format);
    }
}

using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Abstract.Social
{
    public interface IChatFormatter
    {
        /// <summary>
        /// Parse a collection of <see cref="ChatFormat"/>'s from <see cref="IPlayer"/>.
        /// </summary>
        IEnumerable<ChatFormat> ParseChatLinks(IPlayer player, IEnumerable<ChatFormat> formats);

        /// <summary>
        /// Parse a <see cref="ChatFormat"/>.
        /// </summary>
        ChatFormat ParseChatFormat(IPlayer player, ChatFormat format);
    }
}
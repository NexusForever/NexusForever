using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Chat.Shared.Format;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Abstract.Chat.Format
{
    public interface IChatFormatManager
    {
        void Initialise();

        /// <summary>
        /// Converts a collection of <see cref="ChatFormat"/> to a collection of <see cref="ChatChannelTextFormat"/>.
        /// </summary>
        /// <param name="player">Player the chat formats belong to.</param>
        /// <param name="formats">A collection of <see cref="ChatFormat"/>'s to be converted.</param>
        IEnumerable<ChatChannelTextFormat> ToInternal(IPlayer player, IEnumerable<ChatFormat> formats);

        /// <summary>
        /// Converts a collection of <see cref="ChatChannelTextFormat"/> to a collection of <see cref="ChatFormat"/>.
        /// </summary>
        /// <param name="formats">A collection of <see cref="ChatChannelTextFormat"/>'s to be converted.</param>
        IEnumerable<ChatFormat> ToNetwork(IEnumerable<ChatChannelTextFormat> formats);

        /// <summary>
        /// Converts a collection of <see cref="ChatFormat"/> to a collection of <see cref="ChatFormat"/>.
        /// </summary>
        /// <param name="player">Player the chat formats belong to.</param>
        /// <param name="formats">A collection of <see cref="ChatFormat"/>'s to be converted.</param>
        IEnumerable<ChatFormat> ToLocal(IPlayer player, IEnumerable<ChatFormat> formats);
    }
}

using NexusForever.Game.Static.Social;

namespace NexusForever.Network.World.Social
{
    public interface IChatFormatManager
    {
        void Initialise();

        /// <summary>
        /// Returns a new <see cref="IChatFormat"/> model for supplied <see cref="ChatFormatType"/> type.
        /// </summary>
        IChatFormat NewChatFormatModel(ChatFormatType type);
    }
}
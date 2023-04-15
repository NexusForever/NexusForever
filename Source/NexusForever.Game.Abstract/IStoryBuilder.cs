using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract
{
    public interface IStoryBuilder
    {
        /// <summary>
        /// Sends a story panel to the <see cref="IPlayer"/> based on the provided <see cref="StoryPanelEntry"/>.
        /// </summary>
        void SendStoryPanel(StoryPanelEntry entry, IPlayer player);

        /// <summary>
        /// Sends a story communicator window to the <see cref="IPlayer"/>.
        /// </summary>
        void SendStoryCommunicator(uint textId, uint creatureId, IPlayer player, uint durationMs = 10000, StoryPanelType storyPanelType = StoryPanelType.Default, WindowType windowTypeId = WindowType.LeftAligned, uint soundEventId = 0, byte priority = 0);
    }
}
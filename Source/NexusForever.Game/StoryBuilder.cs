using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Shared;

namespace NexusForever.Game
{
    public class StoryBuilder : Singleton<StoryBuilder>, IStoryBuilder
    {
        /// <summary>
        /// Sends a story panel to the <see cref="IPlayer"/> based on the provided <see cref="StoryPanelEntry"/>.
        /// </summary>
        public void SendStoryPanel(StoryPanelEntry entry, IPlayer player)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            var storyMessage = new StoryMessage
            {
                MsgId       = entry.Id,
                GeneralVoId = entry.SoundEventId
            };
            storyMessage.AddPlayer(BuildPlayer(player));

            player.Session.EnqueueMessageEncrypted(new ServerStoryPanelHide());
            player.Session.EnqueueMessageEncrypted(new ServerStoryPanelShow
            {
                StoryMessage = storyMessage
            });
        }

        /// <summary>
        /// Sends a story communicator window to the <see cref="IPlayer"/>.
        /// </summary>
        public void SendStoryCommunicator(uint textId, uint creatureId, IPlayer player, uint durationMs = 10000, StoryPanelType storyPanelType = StoryPanelType.Default, WindowType windowTypeId = WindowType.LeftAligned, uint soundEventId = 0, byte priority = 0)
        {
            if (textId == 0)
                throw new ArgumentOutOfRangeException(nameof(textId));

            var storyMessage = new StoryMessage
            {
                MsgId = textId
            };
            storyMessage.AddCreature(creatureId);
            storyMessage.AddPlayer(BuildPlayer(player));

            player.Session.EnqueueMessageEncrypted(new ServerStoryCommunicatorShow
            {
                StoryMessage   = storyMessage,
                SoundEventId   = soundEventId > 0 ? soundEventId : creatureId,
                DurationMs     = durationMs,
                StoryPanelType = storyPanelType,
                WindowTypeId   = windowTypeId,
                Priority       = priority
            });
        }

        private StoryMessage.Player BuildPlayer(IPlayer player)
        {
            return new StoryMessage.Player
            {
                PlayerGuid    = player.Guid,
                PlayerName    = player.Name,
                PlayerLevel   = player.Level,
                PlayerGender  = player.Sex,
                PlayerRace    = player.Race,
                PlayerClass   = player.Class,
                PlayerFaction = player.Faction1,
                PlayerPath    = player.Path,
                PlayerTitle   = player.TitleManager.ActiveTitleId
            };
        }
    }
}

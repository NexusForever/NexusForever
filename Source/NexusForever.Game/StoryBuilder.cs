using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Story;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model.Story;
using NexusForever.Shared;

namespace NexusForever.Game
{
    public sealed class StoryBuilder : Singleton<StoryBuilder>, IStoryBuilder
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
                MsgId            = entry.Id,
                RandomTextLineId = entry.SoundEventId
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
        public void SendStoryCommunicator(uint textId, uint creatureId, IPlayer player, uint durationMs = 10000, uint soundEventId = 0,
                                        CommunicatorOverlay overlay = CommunicatorOverlay.Default, 
                                        CommunicatorPortraitPlacement placement = CommunicatorPortraitPlacement.Left, 
                                        CommunicatorBackground background = CommunicatorBackground.Default)
        {
            if (textId == 0)
                throw new ArgumentOutOfRangeException(nameof(textId));

            var storyMessage = new StoryMessage
            {
                MsgId = textId
            };
            storyMessage.AddCreature(creatureId);
            storyMessage.AddPlayer(BuildPlayer(player));

            player.Session.EnqueueMessageEncrypted(new ServerStoryTextCommunicator
            {
                StoryMessage      = storyMessage,
                Creature2Id       = soundEventId > 0 ? soundEventId : creatureId,
                DurationMs        = durationMs,
                Overlay           = overlay,
                PortraitPlacement = placement,
                Background        = background
            });
        }

        private StoryMessage.Player BuildPlayer(IPlayer player)
        {
            return new StoryMessage.Player
            {
                UnitId  = player.Guid,
                Name    = player.Name,
                Level   = player.Level,
                Gender  = player.Sex,
                Race    = player.Race,
                Class   = player.Class,
                Faction = player.Faction1,
                Path    = player.Path,
                TitleId   = player.TitleManager.ActiveTitleId
            };
        }
    }
}

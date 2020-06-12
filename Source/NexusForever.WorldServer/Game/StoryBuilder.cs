using System;
using NexusForever.Shared;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Game
{
    public class StoryBuilder : Singleton<StoryBuilder>
    {
        /// <summary>
        /// Sends a story panel to the <see cref="Player"/> based on the provided <see cref="StoryPanelEntry"/>.
        /// </summary>
        public void SendStoryPanel(StoryPanelEntry entry, Player player)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            var storyMessage = new StoryMessage
            {
                MsgId       = entry.Id,
                GeneralVoId = entry.SoundEventId
            };
            storyMessage.AddPlayer(player);

            player.Session.EnqueueMessageEncrypted(new ServerStoryPanelHide());
            player.Session.EnqueueMessageEncrypted(new ServerStoryPanelShow
            {
                StoryMessage = storyMessage
            });
        }

        /// <summary>
        /// Sends a story communicator window to the <see cref="Player"/>.
        /// </summary>
        public void SendStoryCommunicator(uint textId, uint creatureId, Player player, uint durationMs = 10000, StoryPanelType storyPanelType = StoryPanelType.Default, WindowType windowTypeId = WindowType.LeftAligned, uint soundEventId = 0, byte priority = 0)
        {
            if (textId == 0)
                throw new ArgumentOutOfRangeException(nameof(textId));

            var storyMessage = new StoryMessage
            {
                MsgId = textId
            };
            storyMessage.AddCreature(creatureId);
            storyMessage.AddPlayer(player);

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
    }
}

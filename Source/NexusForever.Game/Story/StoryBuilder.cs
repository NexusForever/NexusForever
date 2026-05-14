using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Story;
using NexusForever.Game.Static.Story;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.Session;
using NexusForever.Network.World.Message.Model.Story;

namespace NexusForever.Game.Story
{
    public class StoryBuilder : IStoryBuilder
    {
        #region Dependency Injection

        private readonly IGameTableManager gameTableManager;

        public StoryBuilder(
            IGameTableManager gameTableManager)
        {
            this.gameTableManager = gameTableManager;
        }

        #endregion

        public void SendServerStoryPanelShow(IPlayer player, uint storyPanelId)
        {
            StoryPanelEntry entry = gameTableManager.StoryPanel.GetEntry(storyPanelId);
            if (entry == null)
                return;

            var builder = new StoryMessageBuilder
            {
                MsgId            = entry.Id,
                RandomTextLineId = entry.SoundEventId
            };
            builder.AddPlayer(player);

            player.Session.EnqueueMessageEncrypted(new ServerStoryPanelHide());
            SendServerStoryPanelShow(player.Session, builder);
        }

        public void SendServerStoryPanelShow(IGameSession session, IStoryMessageBuilder builder)
        {
            session.EnqueueMessageEncrypted(new ServerStoryPanelShow
            {
                StoryMessage = builder.StoryMessage
            });
        }

        public void SendGenericFloaterComplex(IGameSession session,
            uint localisedTextId,
            uint randomTextLineId,
            List<IStoryMessageBuilder> storyMessageBuilders)
        {
            var genericFloaterComplex = new ServerGenericFloaterComplex
            {
                LocalisedTextId  = localisedTextId,
                RandomTextLineId = randomTextLineId
            };
            foreach (IStoryMessageBuilder storyMessageBuilder in storyMessageBuilders)
                genericFloaterComplex.Messages.Add(storyMessageBuilder.StoryMessage);

            session.EnqueueMessageEncrypted(genericFloaterComplex);
        }

        public void SendGenericFloaterString(IGameSession session, string text)
        {
            session.EnqueueMessageEncrypted(new ServerGenericFloaterString
            {
                Text = text
            });
        }

        public void SendGenericFloaterLocalised(IGameSession session, uint localisedTextId)
        {
            session.EnqueueMessageEncrypted(new ServerGenericFloaterLocalised
            {
                LocalisedTextId = localisedTextId
            });
        }

        public void SendStoryPanelCustom(IGameSession session,
            IStoryMessageBuilder builder,
            StoryPanelType type,
            StoryPanelStyle style,
            TimeSpan duration,
            uint soundContextEventId)
        {
            session.EnqueueMessageEncrypted(new ServerStoryPanelCustomShow
            {
                StoryMessage        = builder.StoryMessage,
                SoundContextEventId = soundContextEventId,
                StoryPanelType      = type,
                StoryPanelStyle     = style,
                DurationMS          = (uint)duration.TotalMilliseconds
            });
        }

        public void SendServerStoryTextCommunicator(IPlayer player,
            uint localisedTextId,
            uint creatureId,
            TimeSpan duration,
            CommunicatorOverlay overlay,
            CommunicatorPortraitPlacement placement,
            CommunicatorBackground background)
        {
            var builder = new StoryMessageBuilder
            {
                MsgId = localisedTextId
            };
            builder.AddCreature(creatureId);
            builder.AddPlayer(player);

            SendServerStoryTextCommunicator(player.Session, builder, creatureId, duration, overlay, placement, background);
        }

        public void SendServerStoryTextCommunicator(
            IGameSession session,
            IStoryMessageBuilder builder,
            uint creatureId,
            TimeSpan duration,
            CommunicatorOverlay overlay,
            CommunicatorPortraitPlacement placement,
            CommunicatorBackground background)
        {
            session.EnqueueMessageEncrypted(new ServerStoryTextCommunicator
            {
                StoryMessage      = builder.StoryMessage,
                Creature2Id       = creatureId,
                DurationMs        = (uint)duration.TotalMilliseconds,
                Overlay           = overlay,
                PortraitPlacement = placement,
                Background        = background
            });
        }
    }
}

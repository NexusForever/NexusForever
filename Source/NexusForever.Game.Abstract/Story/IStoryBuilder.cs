using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Story;
using NexusForever.Network.Session;

namespace NexusForever.Game.Abstract.Story
{
    public interface IStoryBuilder
    {
        void SendServerStoryPanelShow(IPlayer player, uint storyPanelId);

        void SendServerStoryPanelShow(IGameSession session, IStoryMessageBuilder builder);

        void SendGenericFloaterComplex(IGameSession session,
            uint localisedTextId,
            uint randomTextLineId,
            List<IStoryMessageBuilder> storyMessageBuilders);

        void SendGenericFloaterString(IGameSession session, string text);

        void SendGenericFloaterLocalised(IGameSession session, uint localisedStringId);

        void SendStoryPanelCustom(IGameSession session,
            IStoryMessageBuilder builder,
            StoryPanelType type,
            StoryPanelStyle style,
            TimeSpan duration,
            uint soundContextEventId);

        public void SendServerStoryTextCommunicator(IPlayer player,
            uint localisedTextId,
            uint creatureId,
            TimeSpan duration,
            CommunicatorOverlay overlay,
            CommunicatorPortraitPlacement placement,
            CommunicatorBackground background);

        void SendServerStoryTextCommunicator(
            IGameSession session,
            IStoryMessageBuilder builder,
            uint creatureId,
            TimeSpan duration,
            CommunicatorOverlay overlay,
            CommunicatorPortraitPlacement portraitPlacement,
            CommunicatorBackground background);
    }
}

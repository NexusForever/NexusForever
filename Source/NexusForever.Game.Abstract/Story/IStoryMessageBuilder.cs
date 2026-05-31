using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.World.Message.Model.Story.Message;

namespace NexusForever.Game.Abstract.Story
{
    public interface IStoryMessageBuilder
    {
        StoryMessage StoryMessage { get; }

        void AddCreature(uint creatureId);

        void AddCustomText(string text);

        void AddLocalisedText(uint localisedStringId);

        void AddPlayer(IPlayer player);

        void AddCreatureUnit(uint unitId, uint creatureId);

        void AddPlayerSelf();
    }
}

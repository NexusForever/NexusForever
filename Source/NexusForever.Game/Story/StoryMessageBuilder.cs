using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Story;
using NexusForever.Network.World.Message.Model.Story.Message;

namespace NexusForever.Game.Story
{
    public class StoryMessageBuilder : IStoryMessageBuilder
    {
        public StoryMessage StoryMessage { get; private set; } = new StoryMessage();

        public uint MsgId
        {
            get => StoryMessage.MsgId;
            set => StoryMessage.MsgId = value;
        }

        public uint RandomTextLineId
        {
            get => StoryMessage.RandomTextLineId;
            set => StoryMessage.RandomTextLineId = value;
        }

        public void AddCreature(uint creatureId)
        {
            StoryMessage.Actors.Add(new CreatureActor
            {
                Creature2Id = creatureId
            });
        }

        public void AddCustomText(string text)
        {
            StoryMessage.Actors.Add(new CustomTextActor
            {
                Text = text
            });
        }

        public void AddLocalisedText(uint localisedTextId)
        {
            StoryMessage.Actors.Add(new LocalisedTextActor
            {
                LocalisedTextId = localisedTextId
            });
        }

        public void AddPlayer(IPlayer player)
        {
            StoryMessage.Actors.Add(new PlayerActor
            {
                UnitId  = player.Guid,
                Name    = player.Name,
                Level   = player.Level,
                Gender  = player.Sex,
                Race    = player.Race,
                Class   = player.Class,
                Faction = player.Faction1,
                Path    = player.Path,
                TitleId = player.TitleManager.ActiveTitleId
            });
        }

        public void AddCreatureUnit(uint unitId, uint creatureId)
        {
            StoryMessage.Actors.Add(new CreatureUnitActor
            {
                UnitId      = unitId,
                Creature2Id = creatureId
            });
        }

        public void AddPlayerSelf()
        {
            StoryMessage.Actors.Add(new PlayerSelfActor());
        }
    }
}

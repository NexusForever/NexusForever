using NexusForever.Game.Static.Story;

namespace NexusForever.Network.World.Message.Model.Story.Message
{
    public sealed class CreatureUnitActor : Actor
    {
        public override StoryTextSourceType Type => StoryTextSourceType.CreatureUnit;
        public uint UnitId { get; set; }
        public uint Creature2Id { get; set; } // used if unit details have not been loaded on client

        public override void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Creature2Id, 18u);
            base.Write(writer);
        }
    }
}

using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Story.Message;

namespace NexusForever.Network.World.Message.Model.Story
{
    [Message(GameMessageOpcode.ServerStoryTextUnitYell)]
    public class ServerStoryTextUnitYell : IWritable
    {
        public StoryMessage StoryMessage { get; set; }
        public uint UnitId { get; set; }
        public uint Creature2Id { get; set; } // uses this if Unit has not been loaded to client yet
        public float TextBubbleRange { get; set; }

        public void Write(GamePacketWriter writer)
        {
            StoryMessage.Write(writer);
            writer.Write(UnitId);
            writer.Write(Creature2Id, 18u);
            writer.Write(TextBubbleRange);
        }
    }
}

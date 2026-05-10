using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Story.Message;

namespace NexusForever.Network.World.Message.Model.Story
{
    // If neither IsEmote or IsTextBubble is set, the text is made and NPC Say in the chat log with a chat bubble
    // If either is set, no NPC say is created. Both IsEmote and IsTextBubble can be set at the same time.
    [Message(GameMessageOpcode.ServerStoryTextUnit)]
    public class ServerStoryTextUnit : IWritable
    {
        public StoryMessage StoryMessage { get; set; }
        public uint UnitId { get; set; }
        public bool IsEmote { get; set; }
        public bool IsTextBubble { get; set; }
        public float TextBubbleRange { get; set; }

        public void Write(GamePacketWriter writer)
        {
            StoryMessage.Write(writer);
            writer.Write(UnitId);
            writer.Write(IsEmote);
            writer.Write(IsTextBubble);
            writer.Write(TextBubbleRange);
        }
    }
}

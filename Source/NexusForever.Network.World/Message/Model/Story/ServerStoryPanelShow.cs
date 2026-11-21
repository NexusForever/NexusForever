using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Story.Message;

namespace NexusForever.Network.World.Message.Model.Story
{
    [Message(GameMessageOpcode.ServerStoryPanelShow)]
    public class ServerStoryPanelShow : IWritable
    {
        public StoryMessage StoryMessage { get; set; }

        public void Write(GamePacketWriter writer)
        {
            StoryMessage.Write(writer);
        }
    }
}

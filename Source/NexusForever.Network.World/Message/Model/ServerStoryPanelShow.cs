using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
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

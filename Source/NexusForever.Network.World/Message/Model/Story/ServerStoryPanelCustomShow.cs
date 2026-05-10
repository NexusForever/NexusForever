using NexusForever.Game.Static.Story;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Story.Message;

namespace NexusForever.Network.World.Message.Model.Story
{
    [Message(GameMessageOpcode.ServerStoryPanelCustomShow)]
    public class ServerStoryPanelCustomShow : IWritable
    {
        public StoryMessage StoryMessage { get; set; }
        public uint SoundContextEventId { get; set; }
        public StoryPanelType StoryPanelType { get; set; }
        public uint DurationMS { get; set; }
        public StoryPanelStyle StoryPanelStyle { get; set; }

        public void Write(GamePacketWriter writer)
        {
            StoryMessage.Write(writer);
            writer.Write(SoundContextEventId);
            writer.Write(StoryPanelType, 32u);
            writer.Write(DurationMS);
            writer.Write(StoryPanelStyle, 32u);
        }
    }
}

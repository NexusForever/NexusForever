using NexusForever.Game.Static.Story;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Story
{
    [Message(GameMessageOpcode.ServerStoryTextCommunicator)]
    public class ServerStoryTextCommunicator : IWritable
    {
        public StoryMessage StoryMessage { get; set; }
        public uint Creature2Id { get; set; }
        public uint DurationMs { get; set; } = 10000;
        public CommunicatorPortraitPlacement PortraitPlacement { get; set; }
        public CommunicatorOverlay Overlay { get; set; }
        public CommunicatorBackground Background { get; set; }

        public void Write(GamePacketWriter writer)
        {
            StoryMessage.Write(writer);
            writer.Write(Creature2Id, 18u);
            writer.Write(DurationMs);
            writer.Write(PortraitPlacement, 2u);
            writer.Write(Overlay, 2u);
            writer.Write(Background, 3u);
        }
    }
}

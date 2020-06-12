using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerStoryCommunicatorShow)]
    public class ServerStoryCommunicatorShow : IWritable
    {
        public StoryMessage StoryMessage { get; set; }
        public uint SoundEventId { get; set; } // 18
        public uint DurationMs { get; set; } = 10000;
        public WindowType WindowTypeId { get; set; } = 0; // 2 
        public StoryPanelType StoryPanelType { get; set; } = 0; // 2
        public byte Priority { get; set; } // 3

        public void Write(GamePacketWriter writer)
        {
            StoryMessage.Write(writer);
            writer.Write(SoundEventId, 18u);
            writer.Write(DurationMs);
            writer.Write(WindowTypeId, 2u);
            writer.Write(StoryPanelType, 2u);
            writer.Write(Priority, 3u);
        }
    }
}

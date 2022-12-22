using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerStoryPanelHide)]
    public class ServerStoryPanelHide : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
        }
    }
}

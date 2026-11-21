using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{

    [Message(GameMessageOpcode.ServerPathRefresh)]
    public class ServerPathRefresh : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // zero byte message
        }
    }
}

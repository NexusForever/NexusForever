using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Datacube
{
    [Message(GameMessageOpcode.ServerDatacubeClearAll)]
    public class ServerDatacubeClearAll : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // zero byte message
        }
    }
}

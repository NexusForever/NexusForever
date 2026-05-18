using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerDatacubeUpdate)]
    public class ServerDatacubeUpdate : IWritable
    {
        public Datacube DatacubeData { get; set; }

        public void Write(GamePacketWriter writer)
        {
            DatacubeData.Write(writer);
        }
    }
}

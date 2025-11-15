using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Datacube
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

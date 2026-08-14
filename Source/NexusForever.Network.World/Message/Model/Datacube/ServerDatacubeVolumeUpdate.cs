using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Datacube
{
    [Message(GameMessageOpcode.ServerDatacubeVolumeUpdate)]
    public class ServerDatacubeVolumeUpdate : IWritable
    {
        public Datacube DatacubeVolumeData { get; set; }

        public void Write(GamePacketWriter writer)
        {
            DatacubeVolumeData.Write(writer);
        }
    }
}

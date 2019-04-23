using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerDatacubeUpdateList)]
    public class ServerDatacubeUpdateList : IWritable
    {
        public List<Datacube> DatacubeData { get; set; } = new List<Datacube>();
        public List<Datacube> DatacubeVolumeData { get; set; } = new List<Datacube>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(DatacubeData.Count);
            DatacubeData.ForEach(d => d.Write(writer));

            writer.Write(DatacubeVolumeData.Count);
            DatacubeVolumeData.ForEach(d => d.Write(writer));
        }
    }
}

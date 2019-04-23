using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
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

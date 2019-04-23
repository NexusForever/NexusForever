using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
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

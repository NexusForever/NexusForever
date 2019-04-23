using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class Datacube : IWritable
    {
        public ushort DatacubeId { get; set; }
        public uint Progress { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(DatacubeId, 14u);
            writer.Write(Progress);
        }
    }
}

using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Group
{
    public class GroupMarkerInfo : IWritable
    {
        public MarkerInfo[] Markers { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Markers.Length);
            foreach (MarkerInfo markerInfo in Markers)
                markerInfo.Write(writer);
        }
    }
}

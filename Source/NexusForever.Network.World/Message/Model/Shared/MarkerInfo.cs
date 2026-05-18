using NexusForever.Network.Message;
using NexusForever.Game.Static.Group;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class MarkerInfo : IWritable
    {
        public uint UnitId { get; set; }

        public GroupMarker Marker { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Marker, 32u);
            writer.Write(UnitId);
        }
    }
}

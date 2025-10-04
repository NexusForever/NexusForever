using NexusForever.Game.Static.Group;

namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupMarkerUpdatedMessage
    {
        public Shared.Group Group { get; set; }
        public GroupMarker Marker { get; set; }
        public uint? UnitId { get; set; }
    }
}

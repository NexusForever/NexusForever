using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupMarkerMessage
    {
        public Identity MarkerIndentity { get; set; }
        public GroupMarker GroupMarker { get; set; }
        public uint? UnitId { get; set; }
    }
}

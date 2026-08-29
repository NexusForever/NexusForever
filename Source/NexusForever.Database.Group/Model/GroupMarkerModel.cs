using NexusForever.Game.Static.Group;

namespace NexusForever.Database.Group.Model
{
    public class GroupMarkerModel
    {
        public ulong GroupId { get; set; }
        public GroupMarker Marker { get; set; }
        public uint UnitId { get; set; }

        public GroupModel Group { get; set; }
    }
}

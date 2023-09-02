using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NetworkGroupMarkerInfo = NexusForever.Network.World.Message.Model.Shared.GroupMarkerInfo;

namespace NexusForever.Game.Group
{
    public class GroupMarkerInfo : IGroupMarkerInfo
    {
        private Dictionary<GroupMarker, uint> usedGroupMarkers;
        private Dictionary<uint, GroupMarker> unitIdUsedMarkers;

        public IGroup Group { get; private set; }

        public GroupMarkerInfo(IGroup group)
        {
            Group = group;
            usedGroupMarkers = new Dictionary<GroupMarker, uint>();
            unitIdUsedMarkers = new Dictionary<uint, GroupMarker>();
        }

        public void MarkTarget(uint unitId, GroupMarker marker)
        {
            // client send a UnitId of 0 when it wants to "unmark" somone who is marked.
            if (unitId > 0)
            {
                Unmark(marker);
                Unmark(unitId);
                Mark(unitId, marker);
            }
            else
            {
                Unmark(marker);
            }

            BroadcastMarkEvent(unitId, marker);
        }

        private void BroadcastMarkEvent(uint unitID, GroupMarker marker)
        {
            Group.BroadcastPacket(new ServerGroupMarkUnit
            {
                GroupId = Group.Id,
                Marker = marker,
                UnitId = unitID
            });
        }
        private void Mark(uint unitId, GroupMarker marker)
        {
            usedGroupMarkers.Add(marker, unitId);
            unitIdUsedMarkers.Add(unitId, marker);

            BroadcastMarkEvent(unitId, marker);
        }

        private void Unmark(GroupMarker marker)
        {
            if (!usedGroupMarkers.ContainsKey(marker))
                return;

            usedGroupMarkers.TryGetValue(marker, out uint unitId);
            usedGroupMarkers.Remove(marker);
            unitIdUsedMarkers.Remove(unitId);

            BroadcastMarkEvent(0, marker);
        }

        private void Unmark(uint unitId)
        {
            if (!unitIdUsedMarkers.ContainsKey(unitId))
                return;

            unitIdUsedMarkers.TryGetValue(unitId, out GroupMarker marker);
            usedGroupMarkers.Remove(marker);
            unitIdUsedMarkers.Remove(unitId);

            BroadcastMarkEvent(0, marker);
        }

        public NetworkGroupMarkerInfo Build()
        {
            return new NetworkGroupMarkerInfo
            {
                Markers = usedGroupMarkers.Select(kvp => new MarkerInfo() { Marker = kvp.Key, UnitId = kvp.Value }).ToArray()
            };
        }
    }
}

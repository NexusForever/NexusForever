using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Group
{
    public interface IGroupMarkerInfo : INetworkBuildable<GroupMarkerInfo>
    {
        IGroup Group { get; }

        void MarkTarget(uint unitId, GroupMarker marker);
    }
}
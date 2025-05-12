using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSettlerBuildStatusUpdateList)]
    public class ServerPathSettlerBuildStatusUpdateList : IWritable
    {
        public ushort PathSettlerHubId { get; set; } 
        public List<SettlerImprovementGroupStatus> SettlerImprovementStatuses { get; set; } = new List<SettlerImprovementGroupStatus>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathSettlerHubId, 14);
            writer.Write(SettlerImprovementStatuses.Count);
            foreach (var improvement in SettlerImprovementStatuses)
            {
                improvement.Write(writer);
            }
        }
    }
}

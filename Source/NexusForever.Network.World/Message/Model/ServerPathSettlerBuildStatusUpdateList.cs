using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSettlerBuildStatusUpdateList)]
    public class ServerPathSettlerBuildStatusUpdateList : IWritable
    {
        public class SettlerImprovementGroupStatus
        {
            public ushort PathSettlerImprovementGroupId { get; set; }
            public uint CurrentTier { get; set; }
            public uint RemainingTime { get; set; }
            public uint Unknown { get; set; }
        }

        public ushort PathSettlerHubId { get; set; } 
        public List<SettlerImprovementGroupStatus> SettlerImprovementStatuses { get; set; } = new List<SettlerImprovementGroupStatus>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathSettlerHubId, 14);
            writer.Write(SettlerImprovementStatuses.Count);
            foreach (var improvement in SettlerImprovementStatuses)
            {
                writer.Write(improvement.PathSettlerImprovementGroupId, 14);
                writer.Write(improvement.CurrentTier);
                writer.Write(improvement.RemainingTime);
                writer.Write(improvement.Unknown);
            }
        }
    }
}

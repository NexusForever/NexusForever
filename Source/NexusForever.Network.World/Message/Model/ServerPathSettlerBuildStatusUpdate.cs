using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSettlerBuildStatusUpdate)]
    public class ServerPathSettlerBuildStatusUpdate : IWritable
    {
        public class SettlerImprovementGroupStatus
        {
            public ushort PathSettlerImprovementGroupId { get; set; }
            public uint CurrentTier { get; set; }
            public uint RemainingTime { get; set; }
            public uint Unknown { get; set; }
        }

        public ushort PathSettlerHubId { get; set; }
        public SettlerImprovementGroupStatus Status { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathSettlerHubId, 14);

            writer.Write(Status.PathSettlerImprovementGroupId, 14);
            writer.Write(Status.CurrentTier);
            writer.Write(Status.RemainingTime);
            writer.Write(Status.Unknown);
        }
    }
}

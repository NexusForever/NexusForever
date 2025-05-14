using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class SettlerImprovementGroupStatus : IWritable
    {
        public ushort PathSettlerImprovementGroupId { get; set; }
        public uint CurrentTier { get; set; }
        public uint RemainingTime { get; set; }
        public uint Unknown { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathSettlerImprovementGroupId, 14);
            writer.Write(CurrentTier);
            writer.Write(RemainingTime);
            writer.Write(Unknown);
        }
    }
}

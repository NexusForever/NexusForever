using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    public class SettlerImprovementGroupStatus : IWritable
    {
        public ushort PathSettlerImprovementGroupId { get; set; }
        public int Tier { get; set; } // -1 seems to mean the improvement is inactive
        public uint RemainingTimeMS { get; set; }
        public uint BundleCount { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathSettlerImprovementGroupId, 14);
            writer.Write(Tier);
            writer.Write(RemainingTimeMS);
            writer.Write(BundleCount);
        }
    }
}

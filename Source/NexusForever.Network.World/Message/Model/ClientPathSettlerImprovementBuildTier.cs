using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPathSettlerImprovementBuildTier)]
    public class ClientPathSettlerImprovementBuildTier : IReadable
    {
        public uint PathSettlerImprovementGroupId { get; private set; }
        public uint BuildTier { get; private set; } // Is zero-based, so 0 = 1st tier, 1 = 2nd tier, etc.

        public void Read(GamePacketReader reader)
        {
            PathSettlerImprovementGroupId = reader.ReadUInt(14);
            BuildTier = reader.ReadUInt();
        }
    }
}

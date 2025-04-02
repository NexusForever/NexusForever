using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPathSettlerImprovement_BuildTier)]
    public class ClientPathSettlerImprovement_BuildTier : IReadable
    {
        public uint PathSettlerImprovementGroupId { get; set; }
        public uint BuildTier { get; set; } // Is zero-based, so 0 = 1st tier, 1 = 2nd tier, etc.

        public void Read(GamePacketReader reader)
        {
            PathSettlerImprovementGroupId = reader.ReadUInt(14);
            BuildTier = reader.ReadUInt();
        }
    }
}

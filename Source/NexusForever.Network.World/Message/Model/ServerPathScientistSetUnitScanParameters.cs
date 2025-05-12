using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathScientistSetUnitScanParameters)]
    public class ServerPathScientistSetUnitScanParameters : IWritable
    {
        public enum ScanReward //Bit flags. Meanings a bit speculative.
        {
            RewardForScan = 0x1,
            RewardForRawScan = 0x2,
            SpellBuffForScan = 0x4,
            SpecimenSurveyReward = 0x8,
            HasLoot = 0x10
        }

        public uint UnitId { get; set; }
        public uint ScanRewardFlags { get; set; } 
        public bool IsScannable { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(ScanRewardFlags);
            writer.Write(IsScannable);
        }
    }
}

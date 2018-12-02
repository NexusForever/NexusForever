namespace NexusForever.Shared.GameTable.Model
{
    public class TextEntry
    {
        public uint Id;
        public string LocalizedText;
    }
    public class XpPerLevelEntry
    {
        public uint Id;
        public uint MinXpForLevel;
        public uint BaseQuestXpPerLevel;
        public uint AbilityPointsPerLevel;
        public uint AttributePointsPerLevel;
        public uint BaseRepRewardPerLevel;
    }
}

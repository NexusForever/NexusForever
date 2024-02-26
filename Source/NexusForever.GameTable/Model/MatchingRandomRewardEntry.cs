namespace NexusForever.GameTable.Model
{
    public class MatchingRandomRewardEntry
    {
        public uint Id { get; set; }
        public uint MatchTypeEnum { get; set; }
        public uint Item2Id { get; set; }
        public uint ItemCount { get; set; }
        public uint CurrencyTypeId { get; set; }
        public uint CurrencyValue { get; set; }
        public uint XpEarned { get; set; }
        public uint Item2IdLevelScale { get; set; }
        public uint ItemCountLevelScale { get; set; }
        public uint CurrencyTypeIdLevelScale { get; set; }
        public uint CurrencyValueLevelScale { get; set; }
        public uint XpEarnedLevelScale { get; set; }
        public uint WorldDifficultyEnum { get; set; }
    }
}

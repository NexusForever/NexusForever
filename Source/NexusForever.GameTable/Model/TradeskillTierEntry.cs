namespace NexusForever.GameTable.Model
{
    public class TradeskillTierEntry
    {
        public uint Id { get; set; }
        public uint TradeSkillId { get; set; }
        public uint Tier { get; set; }
        public uint RequiredXp { get; set; }
        public uint LearnXp { get; set; }
        public uint CraftXp { get; set; }
        public uint FirstCraftXp { get; set; }
        public uint QuestXp { get; set; }
        public uint FailXp { get; set; }
        public uint ItemLevelMin { get; set; }
        public uint MaxAdditives { get; set; }
        public ulong RelearnCost { get; set; }
        public uint AchievementCategoryId { get; set; }
    }
}

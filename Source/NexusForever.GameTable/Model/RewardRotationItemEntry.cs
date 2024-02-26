namespace NexusForever.GameTable.Model
{
    public class RewardRotationItemEntry
    {
        public uint Id { get; set; }
        public uint RewardItemTypeEnum { get; set; }
        public uint RewardItemObject { get; set; }
        public uint Count { get; set; }
        public string IconPath { get; set; }
        public uint MinPlayerLevel { get; set; }
        public uint WorldDifficultyFlags { get; set; }
    }
}

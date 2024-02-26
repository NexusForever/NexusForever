namespace NexusForever.GameTable.Model
{
    public class RewardRotationModifierEntry
    {
        public uint Id { get; set; }
        public uint RewardPropertyId { get; set; }
        public uint RewardPropertyData { get; set; }
        public float Value { get; set; }
        public uint MinPlayerLevel { get; set; }
        public uint WorldDifficultyFlags { get; set; }
    }
}

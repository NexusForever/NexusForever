namespace NexusForever.GameTable.Model
{
    public class RewardPropertyPremiumModifierEntry
    {
        public uint Id { get; set; }
        public uint PremiumSystemEnum { get; set; }
        public uint Tier { get; set; }
        public uint RewardPropertyId { get; set; }
        public uint RewardPropertyData { get; set; }
        public uint ModifierValueInt { get; set; }
        public float ModifierValueFloat { get; set; }
        public uint EntitlementIdModifierCount { get; set; }
        public uint Flags { get; set; }
    }
}

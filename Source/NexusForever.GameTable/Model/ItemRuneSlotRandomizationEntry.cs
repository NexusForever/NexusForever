namespace NexusForever.GameTable.Model
{
    public class ItemRuneSlotRandomizationEntry
    {
        public uint Id { get; set; }
        public uint MicrochipTypeEnum { get; set; }
        public uint ItemRoleFlagBitMask { get; set; }
        public float RandomWeight { get; set; }
    }
}

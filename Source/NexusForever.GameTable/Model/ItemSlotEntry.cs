namespace NexusForever.GameTable.Model
{
    public class ItemSlotEntry
    {
        public uint Id { get; set; }
        public string EnumName { get; set; }
        public uint EquippedSlotFlags { get; set; }
        public float ArmorModifier { get; set; }
        public float ItemLevelModifier { get; set; }
        public uint SlotBonus { get; set; }
        public uint GlyphSlotBonus { get; set; }
        public uint MinLevel { get; set; }
    }
}

using NexusForever.GameTable.Static { get; set; }

namespace NexusForever.GameTable.Model
{
    public class UnitProperty2Entry
    {
        public uint Id { get; set; }
        public string Description { get; set; }
        public string EnumName { get; set; }
        public float DefaultValue { get; set; }
        public uint LocalizedTextId { get; set; }
        public float ValuePerPoint { get; set; }
        public UnitPropertyFlags Flags { get; set; }
        public uint TooltipDisplayOrder { get; set; }
        public uint ProfiencyFlagEnum { get; set; }
        public uint ItemCraftingGroupFlagBitMask { get; set; }
        public uint EquippedSlotFlags { get; set; }
    }
}

using NexusForever.GameTable.Static { get; set; }

namespace NexusForever.GameTable.Model
{
    public class Item2CategoryEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextId { get; set; }
        public uint ItemProficiencyId { get; set; }
        public SecondaryItemFlags Flags { get; set; }
        public uint TradeSkillId { get; set; }
        public uint SoundEventIdEquip { get; set; }
        public float VendorMultiplier { get; set; }
        public float TurninMultiplier { get; set; }
        public float ArmorModifier { get; set; }
        public float ArmorBase { get; set; }
        public float WeaponPowerModifier { get; set; }
        public uint WeaponPowerBase { get; set; }
        public uint Item2FamilyId { get; set; }
    }
}

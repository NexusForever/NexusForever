using NexusForever.GameTable.Static;

namespace NexusForever.GameTable.Model
{
    public class Item2CategoryEntry
    {
        public uint Id;
        public uint LocalizedTextId;
        public uint ItemProficiencyId;
        public SecondaryItemFlags Flags;
        public uint TradeSkillId;
        public uint SoundEventIdEquip;
        public float VendorMultiplier;
        public float TurninMultiplier;
        public float ArmorModifier;
        public float ArmorBase;
        public float WeaponPowerModifier;
        public uint WeaponPowerBase;
        public uint Item2FamilyId;
    }
}

using NexusForever.Game.Static.Entity { get; set; }
using NexusForever.GameTable.Static { get; set; }

namespace NexusForever.GameTable.Model
{
    public class Item2Entry
    {
        public uint Id { get; set; }
        public uint ItemBudgetId { get; set; }
        public uint ItemStatId { get; set; }
        public uint ItemRuneInstanceId { get; set; }
        public uint ItemQualityId { get; set; }
        public uint ItemSpecialId00 { get; set; }
        public uint ItemImbuementId { get; set; }
        public uint Item2FamilyId { get; set; }
        public uint Item2CategoryId { get; set; }
        public uint Item2TypeId { get; set; }
        public uint ItemDisplayId { get; set; }
        public uint ItemSourceId { get; set; }
        public uint ClassRequired { get; set; }
        public uint RaceRequired { get; set; }
        public uint Faction2IdRequired { get; set; }
        public uint PowerLevel { get; set; }
        public uint RequiredLevel { get; set; }
        public uint RequiredItemLevel { get; set; }
        public uint PrerequisiteId { get; set; }
        public uint EquippedSlotFlags { get; set; }
        public uint MaxStackCount { get; set; }
        public uint MaxCharges { get; set; }
        public uint ExpirationTimeMinutes { get; set; }
        public uint Quest2IdActivation { get; set; }
        public uint Quest2IdActivationRequired { get; set; }
        public uint QuestObjectiveActivationRequired { get; set; }
        public uint TradeskillAdditiveId { get; set; }
        public uint TradeskillCatalystId { get; set; }
        public uint HousingDecorInfoId { get; set; }
        public uint HousingWarplotBossTokenId { get; set; }
        public uint GenericUnlockSetId { get; set; }
        public ItemFlags Flags { get; set; }
        public uint BindFlags { get; set; }
        public uint BuyFromVendorStackCount { get; set; }
        [GameTableFieldArray(2)]
        public CurrencyType[] CurrencyTypeId { get; set; }
        [GameTableFieldArray(2)]
        public uint[] CurrencyAmount { get; set; }
        [GameTableFieldArray(2)]
        public CurrencyType[] CurrencyTypeIdSellToVendor { get; set; }
        [GameTableFieldArray(2)]
        public uint[] CurrencyAmountSellToVendor { get; set; }
        public uint ItemColorSetId { get; set; }
        public float SupportPowerPercentage { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint LocalizedTextIdTooltip { get; set; }
        public string ButtonTemplate { get; set; }
        public string ButtonIcon { get; set; }
        public uint SoundEventIdEquip { get; set; }
    }
}

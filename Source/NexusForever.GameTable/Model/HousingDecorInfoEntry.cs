namespace NexusForever.GameTable.Model
{
    public class HousingDecorInfoEntry
    {
        public uint Id { get; set; }
        public uint HousingDecorTypeId { get; set; }
        public uint HookTypeId { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint Flags { get; set; }
        public uint HookAssetId { get; set; }
        public uint Cost { get; set; }
        public uint CostCurrencyTypeId { get; set; }
        public uint Creature2IdActiveProp { get; set; }
        public uint PrerequisiteIdUnlock { get; set; }
        public uint Spell4IdInteriorBuff { get; set; }
        public uint HousingDecorLimitCategoryId { get; set; }
        public string AltPreviewAsset { get; set; }
        public string AltEditAsset { get; set; }
        public float MinScale { get; set; }
        public float MaxScale { get; set; }
    }
}

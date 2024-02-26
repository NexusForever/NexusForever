namespace NexusForever.GameTable.Model
{
    public class HousingWallpaperInfoEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextId { get; set; }
        public uint Cost { get; set; }
        public uint CostCurrencyTypeId { get; set; }
        public uint ReplaceableMaterialInfoId { get; set; }
        public uint WorldSkyId { get; set; }
        public uint Flags { get; set; }
        public uint PrerequisiteIdUnlock { get; set; }
        public uint PrerequisiteIdUse { get; set; }
        public uint UnlockIndex { get; set; }
        public uint SoundZoneKitId { get; set; }
        public uint WorldLayerId00 { get; set; }
        public uint WorldLayerId01 { get; set; }
        public uint WorldLayerId02 { get; set; }
        public uint WorldLayerId03 { get; set; }
        public uint AccountItemIdUpsell { get; set; }
    }
}

namespace NexusForever.GameTable.Model
{
    public class RaceEntry
    {
        public uint Id { get; set; }
        public string EnumName { get; set; }
        public uint LocalizedTextId { get; set; }
        public uint LocalizedTextIdNameFemale { get; set; }
        public string MaleAssetPath { get; set; }
        public string FemaleAssetPath { get; set; }
        public float HitRadius { get; set; }
        public uint SoundImpactDescriptionIdOrigin { get; set; }
        public uint SoundImpactDescriptionIdTarget { get; set; }
        public float WalkLand { get; set; }
        public float WalkAir { get; set; }
        public float WalkWater { get; set; }
        public float WalkHover { get; set; }
        public uint UnitVisualTypeIdMale { get; set; }
        public uint UnitVisualTypeIdFemale { get; set; }
        public uint SoundEventIdMaleHealthStart { get; set; }
        public uint SoundEventIdFemaleHealthStart { get; set; }
        public uint SoundEventIdMaleHealthStop { get; set; }
        public uint SoundEventIdFemaleHealthStop { get; set; }
        public float SwimWaterDepth { get; set; }
        public uint ItemDisplayIdUnderwearLegsMale { get; set; }
        public uint ItemDisplayIdUnderwearLegsFemale { get; set; }
        public uint ItemDisplayIdUnderwearChestFemale { get; set; }
        public uint ItemDisplayIdArmCannon { get; set; }
        public float MountScaleMale { get; set; }
        public float MountScaleFemale { get; set; }
        public uint SoundSwitchId { get; set; }
        public uint ComponentLayoutIdMale { get; set; }
        public uint ComponentLayoutIdFemale { get; set; }
        public uint ModelMeshIdMountItemMale { get; set; }
        public uint ModelMeshIdMountItemFemale { get; set; }
    }
}

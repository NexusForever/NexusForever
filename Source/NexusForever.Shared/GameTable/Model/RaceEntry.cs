namespace NexusForever.Shared.GameTable.Model
{
    public class RaceEntry
    {
        public uint Id;
        public string EnumName;
        public uint LocalizedTextId;
        public uint LocalizedTextIdNameFemale;
        public string MaleAssetPath;
        public string FemaleAssetPath;
        public float HitRadius;
        public uint SoundImpactDescriptionIdOrigin;
        public uint SoundImpactDescriptionIdTarget;
        public float WalkLand;
        public float WalkAir;
        public float WalkWater;
        public float WalkHover;
        public uint UnitVisualTypeIdMale;
        public uint UnitVisualTypeIdFemale;
        public uint SoundEventIdMaleHealthStart;
        public uint SoundEventIdFemaleHealthStart;
        public uint SoundEventIdMaleHealthStop;
        public uint SoundEventIdFemaleHealthStop;
        public float SwimWaterDepth;
        public uint ItemDisplayIdUnderwearLegsMale;
        public uint ItemDisplayIdUnderwearLegsFemale;
        public uint ItemDisplayIdUnderwearChestFemale;
        public uint ItemDisplayIdArmCannon;
        public float MountScaleMale;
        public float MountScaleFemale;
        public uint SoundSwitchId;
        public uint ComponentLayoutIdMale;
        public uint ComponentLayoutIdFemale;
        public uint ModelMeshIdMountItemMale;
        public uint ModelMeshIdMountItemFemale;
    }
}

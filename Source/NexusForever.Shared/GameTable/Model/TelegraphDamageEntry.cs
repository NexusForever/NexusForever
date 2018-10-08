namespace NexusForever.Shared.GameTable.Model
{
    public class TelegraphDamageEntry
    {
        public uint Id;
        public uint TelegraphSubtypeEnum;
        public uint DamageShapeEnum;
        public float Param00;
        public float Param01;
        public float Param02;
        public float Param03;
        public float Param04;
        public float Param05;
        public uint TelegraphTimeStartMs;
        public uint TelegraphTimeEndMs;
        public uint TelegraphTimeRampInMs;
        public uint TelegraphTimeRampOutMs;
        public float XPositionOffset;
        public float YPositionOffset;
        public float ZPositionOffset;
        public float RotationDegrees;
        public uint TelegraphDamageFlags;
        public uint TargetTypeFlags;
        public uint PhaseFlags;
        public uint PrerequisiteIdCaster;
        public uint SpellThresholdRestrictionFlags;
        public uint DisplayFlags;
        public uint OpacityModifier;
        public uint DisplayGroup;
    }
}

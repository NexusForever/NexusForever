namespace NexusForever.GameTable.Model
{
    public class TelegraphDamageEntry
    {
        public uint Id { get; set; }
        public uint TelegraphSubtypeEnum { get; set; }
        public uint DamageShapeEnum { get; set; }
        public float Param00 { get; set; }
        public float Param01 { get; set; }
        public float Param02 { get; set; }
        public float Param03 { get; set; }
        public float Param04 { get; set; }
        public float Param05 { get; set; }
        public uint TelegraphTimeStartMs { get; set; }
        public uint TelegraphTimeEndMs { get; set; }
        public uint TelegraphTimeRampInMs { get; set; }
        public uint TelegraphTimeRampOutMs { get; set; }
        public float XPositionOffset { get; set; }
        public float YPositionOffset { get; set; }
        public float ZPositionOffset { get; set; }
        public float RotationDegrees { get; set; }
        public uint TelegraphDamageFlags { get; set; }
        public uint TargetTypeFlags { get; set; }
        public uint PhaseFlags { get; set; }
        public uint PrerequisiteIdCaster { get; set; }
        public uint SpellThresholdRestrictionFlags { get; set; }
        public uint DisplayFlags { get; set; }
        public uint OpacityModifier { get; set; }
        public uint DisplayGroup { get; set; }
    }
}

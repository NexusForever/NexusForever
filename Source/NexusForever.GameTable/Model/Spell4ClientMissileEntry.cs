namespace NexusForever.GameTable.Model
{
    public class Spell4ClientMissileEntry
    {
        public uint Id { get; set; }
        public uint MissileType { get; set; }
        public uint CastStage { get; set; }
        public uint OriginUnitEnum { get; set; }
        public uint TargetFlags { get; set; }
        public string ModelPath { get; set; }
        public string FxPath { get; set; }
        public string BeamPath { get; set; }
        public uint BeamSource { get; set; }
        public uint BeamTarget { get; set; }
        public uint ItemSlot { get; set; }
        public uint CostumeSide { get; set; }
        public uint ModelAttachmentIdCaster { get; set; }
        public uint ModelAttachmentIdTarget { get; set; }
        public uint ClientDelay { get; set; }
        public uint ModelEventIdDelayedBy { get; set; }
        public uint Flags { get; set; }
        public uint Duration { get; set; }
        public uint Frequency { get; set; }
        public uint SpeedMps { get; set; }
        public float AccMpss { get; set; }
        public uint RevolverNestedMissileInitDelay { get; set; }
        public uint RevolverNestedMissileSubDelay { get; set; }
        public uint Spell4ClientMissileIdNested { get; set; }
        public string RevolverMissileImpactAssetPath { get; set; }
        public uint MissileRevolverTrackId { get; set; }
        public string BirthAnchorPath { get; set; }
        public string DeathAnchorPath { get; set; }
        public string TrajAnchorPath { get; set; }
        public float BirthDuration { get; set; }
        public float BirthAnchorAngleMin { get; set; }
        public float BirthAnchorAngleMax { get; set; }
        public float DeathAnchorAngleMin { get; set; }
        public float DeathAnchorAngleMax { get; set; }
        public uint DeathAnchorSpace { get; set; }
        public uint ItemSlotIdObj { get; set; }
        public uint ObjCostumeSide { get; set; }
        public float TrajPoseFullBlendDistance { get; set; }
        public float TrajAnchorPlaySpeed { get; set; }
        public float ParabolaHeightScale { get; set; }
        public float RotateX { get; set; }
        public float RotateY { get; set; }
        public float RotateZ { get; set; }
        public float Scale { get; set; }
        public float EndScale { get; set; }
        public uint PhaseFlags { get; set; }
        public uint TelegraphDamageIdAttach { get; set; }
        public uint SoundEventIdBirth { get; set; }
        public uint SoundEventIdLoopStart { get; set; }
        public uint SoundEventIdLoopStop { get; set; }
        public uint SoundEventIdDeath { get; set; }
        public uint BeamDiffuseColor { get; set; }
        public uint MissileDiffuseColor { get; set; }
    }
}

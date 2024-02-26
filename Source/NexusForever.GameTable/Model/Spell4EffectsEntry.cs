namespace NexusForever.GameTable.Model
{
    public class Spell4EffectsEntry
    {
        public uint Id { get; set; }
        public uint SpellId { get; set; }
        public uint TargetFlags { get; set; }
        public uint EffectType { get; set; }
        public uint DamageType { get; set; }
        public uint DelayTime { get; set; }
        public uint TickTime { get; set; }
        public uint DurationTime { get; set; }
        public uint Flags { get; set; }
        public uint DataBits00 { get; set; }
        public uint DataBits01 { get; set; }
        public uint DataBits02 { get; set; }
        public uint DataBits03 { get; set; }
        public uint DataBits04 { get; set; }
        public uint DataBits05 { get; set; }
        public uint DataBits06 { get; set; }
        public uint DataBits07 { get; set; }
        public uint DataBits08 { get; set; }
        public uint DataBits09 { get; set; }
        public uint InnateCostPerTickType0 { get; set; }
        public uint InnateCostPerTickType1 { get; set; }
        public uint InnateCostPerTick0 { get; set; }
        public uint InnateCostPerTick1 { get; set; }
        public uint EmmComparison { get; set; }
        public uint EmmValue { get; set; }
        public float ThreatMultiplier { get; set; }
        public uint Spell4EffectGroupListId { get; set; }
        public uint PrerequisiteIdCasterApply { get; set; }
        public uint PrerequisiteIdTargetApply { get; set; }
        public uint PrerequisiteIdCasterPersistence { get; set; }
        public uint PrerequisiteIdTargetPersistence { get; set; }
        public uint PrerequisiteIdTargetSuspend { get; set; }
        public uint ParameterType00 { get; set; }
        public uint ParameterType01 { get; set; }
        public uint ParameterType02 { get; set; }
        public uint ParameterType03 { get; set; }
        public float ParameterValue00 { get; set; }
        public float ParameterValue01 { get; set; }
        public float ParameterValue02 { get; set; }
        public float ParameterValue03 { get; set; }
        public uint PhaseFlags { get; set; }
        public uint OrderIndex { get; set; }
    }
}

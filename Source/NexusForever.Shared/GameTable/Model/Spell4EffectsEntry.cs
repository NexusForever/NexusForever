using NexusForever.Shared.GameTable.Static;

namespace NexusForever.Shared.GameTable.Model
{
    public class Spell4EffectsEntry
    {
        public uint Id;
        public uint SpellId;
        public uint TargetFlags;
        public uint EffectType;
        public uint DamageType;
        public uint DelayTime;
        public uint TickTime;
        public uint DurationTime;
        public uint Flags;
        [GameTableFieldArray(10)]
        public ICustomGameTableStructure DataBits;
        public uint InnateCostPerTickType0;
        public uint InnateCostPerTickType1;
        public uint InnateCostPerTick0;
        public uint InnateCostPerTick1;
        public uint EmmComparison;
        public uint EmmValue;
        public float ThreatMultiplier;
        public uint Spell4EffectGroupListId;
        public uint PrerequisiteIdCasterApply;
        public uint PrerequisiteIdTargetApply;
        public uint PrerequisiteIdCasterPersistence;
        public uint PrerequisiteIdTargetPersistence;
        public uint PrerequisiteIdTargetSuspend;
        public uint ParameterType00;
        public uint ParameterType01;
        public uint ParameterType02;
        public uint ParameterType03;
        public float ParameterValue00;
        public float ParameterValue01;
        public float ParameterValue02;
        public float ParameterValue03;
        public uint PhaseFlags;
        public uint OrderIndex;
    }
}

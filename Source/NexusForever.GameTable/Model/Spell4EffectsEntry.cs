using NexusForever.Game.Static.Spell;

namespace NexusForever.GameTable.Model
{
    public class Spell4EffectsEntry
    {
        public uint Id;
        public uint SpellId;
        public uint TargetFlags;
        public SpellEffectType EffectType;
        public DamageType DamageType;
        public uint DelayTime;
        public uint TickTime;
        public uint DurationTime;
        public uint Flags;
        public uint DataBits00;
        public uint DataBits01;
        public uint DataBits02;
        public uint DataBits03;
        public uint DataBits04;
        public uint DataBits05;
        public uint DataBits06;
        public uint DataBits07;
        public uint DataBits08;
        public uint DataBits09;
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
        [GameTableFieldArray(4)]
        public uint[] ParameterType;
        [GameTableFieldArray(4)]
        public float[] ParameterValue;
        public uint PhaseFlags;
        public uint OrderIndex;
    }
}

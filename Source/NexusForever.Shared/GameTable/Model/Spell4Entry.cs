namespace NexusForever.Shared.GameTable.Model
{
    public class Spell4Entry
    {
        public uint Id;
        public string Description;
        public uint Spell4BaseIdBaseSpell;
        public uint TierIndex;
        public uint RavelInstanceId;
        public uint CastTime;
        public uint SpellDuration;
        public uint SpellCoolDown;
        public float TargetMinRange;
        public float TargetMaxRange;
        public float TargetVerticalRange;
        [GameTableFieldArray(2u)]
        public uint[] CasterInnateRequirements;
        [GameTableFieldArray(2u)]
        public uint[] CasterInnateRequirementValues;
        [GameTableFieldArray(2u)]
        public uint[] CasterInnateRequirementEval;
        public uint TargetBeginInnateRequirement;
        public uint TargetBeginInnateRequirementValue;
        public uint TargetBeginInnateRequirementEval;
        [GameTableFieldArray(2u)]
        public uint[] InnateCostTypes;
        [GameTableFieldArray(2u)]
        public uint[] InnateCosts;
        [GameTableFieldArray(2u)]
        public uint[] InnateCostEMMIds;
        public uint ChannelInitialDelay;
        public uint ChannelMaxTime;
        public uint ChannelPulseTime;
        public uint LocalizedTextIdActionBarTooltip;
        public uint StackPriority;
        public uint Spell4VisualGroupId;
        public uint Spell4IdCastEvent00;
        public uint Spell4IdCastEvent01;
        public uint Spell4IdCastEvent02;
        public uint Spell4IdCastEvent03;
        public uint Spell4ReagentId00;
        public uint Spell4ReagentId01;
        public uint Spell4ReagentId02;
        public uint Spell4RunnerId00;
        public uint Spell4RunnerId01;
        public uint RunnerTargetTypeEnum00;
        public uint RunnerTargetTypeEnum01;
        [GameTableFieldArray(2u)]
        public uint[] PrerequisiteIdRunners;
        public uint AbilityChargeCount;
        public uint AbilityRechargeTime;
        public uint AbilityRechargeCount;
        public uint ThresholdTime;
        public uint AbilityPointCost;
        public uint TrainingCost;
        public uint SpellChannelFlags;
        public uint IgnoreFollowTimeMs;
        public uint Spell4IdMechanicAlternateSpell;
        public uint Spell4IdPetSwitch;
        public uint Spell4TagId00;
        public uint Spell4TagId01;
        public uint Spell4TagId02;
        public uint Spell4TagId03;
        public uint Spell4TagId04;
        public uint LocalizedTextIdCasterIconSpellText;
        public uint LocalizedTextIdPrimaryTargetIconSpellText;
        public uint LocalizedTextIdSecondaryTargetIconSpellText;
        public uint LocalizedTextIdLASTier;
        public uint LocalizedTextIdTooltipCastInfo;
        public uint LocalizedTextIdTooltipCostInfo;
        public uint TooltipCastTime;
        public uint Spell4AoeTargetConstraintsId;
        public uint Spell4ConditionsIdCaster;
        public uint Spell4ConditionsIdTarget;
        public uint Spell4CCConditionsIdCaster;
        public uint Spell4CCConditionsIdTarget;
        public uint SpellCoolDownIdGlobal;
        public uint SpellCoolDownId00;
        public uint SpellCoolDownId01;
        public uint SpellCoolDownId02;
        public uint Spell4GroupListId;
        public uint MissileSpeed;
        public uint MinMissileSpeed;
        public uint Spell4ClientMissileId00;
        public uint Spell4ClientMissileId01;
        public uint Spell4ClientMissileId02;
        public uint Spell4ClientMissileId03;
        public uint Spell4ClientMissileId04;
        public uint Spell4ClientMissileId05;
        public uint GlobalCooldownEnum;
        public uint PropertyFlags;
        public uint UiFlags;
        public uint Spell4StackGroupId;
        public uint PrerequisiteIdCasterCast;
        public uint PrerequisiteIdTargetCast;
        public uint PrerequisiteIdCasterPersistence;
        public uint PrerequisiteIdTargetPersistence;
        public uint CastEventConditionEnum00;
        public uint CastEventConditionEnum01;
        public uint CastEventConditionEnum02;
        public uint CastEventConditionEnum03;
        public uint CastEventTargetFlags00;
        public uint CastEventTargetFlags01;
        public uint CastEventTargetFlags02;
        public uint CastEventTargetFlags03;
        public uint SpellCastStealthChange;
        public uint PrerequisiteIdAoeTarget;
        public uint PrerequisiteIdAoePreferredTarget;
    }
}

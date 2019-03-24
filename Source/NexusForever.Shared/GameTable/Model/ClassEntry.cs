namespace NexusForever.Shared.GameTable.Model
{
    public class ClassEntry
    {
        public uint Id;
        public string EnumName;
        public uint LocalizedTextId;
        public uint LocalizedTextIdNameFemale;
        public uint Mechanic;

        [GameTableFieldArray(3)]
        public uint[] Spell4IdInnateAbilityActive;

        [GameTableFieldArray(3)]
        public uint[] Spell4IdInnateAbilityPassive;

        [GameTableFieldArray(3)]
        public uint[] PrerequisiteIdInnateAbility;

        public uint StartingItemProficiencies;

        [GameTableFieldArray(2)]
        public uint[] Spell4IdAttackPrimary;

        [GameTableFieldArray(2)]
        public uint[] Spell4IdAttackUnarmed;

        public uint Spell4IdResAbility;
        public uint LocalizedTextIdDescription;
        public uint Spell4GroupId;
        public uint ClassIdForClassApModifier;
        public uint VitalEnumResource00;
        public uint VitalEnumResource01;
        public uint VitalEnumResource02;
        public uint VitalEnumResource03;
        public uint VitalEnumResource04;
        public uint VitalEnumResource05;
        public uint VitalEnumResource06;
        public uint VitalEnumResource07;
        public uint LocalizedTextIdResourceAlert00;
        public uint LocalizedTextIdResourceAlert01;
        public uint LocalizedTextIdResourceAlert02;
        public uint LocalizedTextIdResourceAlert03;
        public uint LocalizedTextIdResourceAlert04;
        public uint LocalizedTextIdResourceAlert05;
        public uint LocalizedTextIdResourceAlert06;
        public uint LocalizedTextIdResourceAlert07;
        public uint AttributeMilestoneGroupId00;
        public uint AttributeMilestoneGroupId01;
        public uint AttributeMilestoneGroupId02;
        public uint AttributeMilestoneGroupId03;
        public uint AttributeMilestoneGroupId04;
        public uint AttributeMilestoneGroupId05;
        public uint ClassSecondaryStatBonusId00;
        public uint ClassSecondaryStatBonusId01;
        public uint ClassSecondaryStatBonusId02;
        public uint ClassSecondaryStatBonusId03;
        public uint ClassSecondaryStatBonusId04;
        public uint ClassSecondaryStatBonusId05;
        public uint AttributeMiniMilestoneGroupId00;
        public uint AttributeMiniMilestoneGroupId01;
        public uint AttributeMiniMilestoneGroupId02;
        public uint AttributeMiniMilestoneGroupId03;
        public uint AttributeMiniMilestoneGroupId04;
        public uint AttributeMiniMilestoneGroupId05;
        public uint AttributeMilestoneMaxTiers00;
        public uint AttributeMilestoneMaxTiers01;
        public uint AttributeMilestoneMaxTiers02;
        public uint AttributeMilestoneMaxTiers03;
        public uint AttributeMilestoneMaxTiers04;
        public uint AttributeMilestoneMaxTiers05;
    }
}

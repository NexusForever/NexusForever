namespace NexusForever.GameTable.Model
{
    public class ClassEntry
    {
        public uint Id { get; set; }
        public string EnumName { get; set; }
        public uint LocalizedTextId { get; set; }
        public uint LocalizedTextIdNameFemale { get; set; }
        public uint Mechanic { get; set; }

        [GameTableFieldArray(3)]
        public uint[] Spell4IdInnateAbilityActive { get; set; }

        [GameTableFieldArray(3)]
        public uint[] Spell4IdInnateAbilityPassive { get; set; }

        [GameTableFieldArray(3)]
        public uint[] PrerequisiteIdInnateAbility { get; set; }

        public uint StartingItemProficiencies { get; set; }

        [GameTableFieldArray(2)]
        public uint[] Spell4IdAttackPrimary { get; set; }

        [GameTableFieldArray(2)]
        public uint[] Spell4IdAttackUnarmed { get; set; }

        public uint Spell4IdResAbility { get; set; }
        public uint LocalizedTextIdDescription { get; set; }
        public uint Spell4GroupId { get; set; }
        public uint ClassIdForClassApModifier { get; set; }
        public uint VitalEnumResource00 { get; set; }
        public uint VitalEnumResource01 { get; set; }
        public uint VitalEnumResource02 { get; set; }
        public uint VitalEnumResource03 { get; set; }
        public uint VitalEnumResource04 { get; set; }
        public uint VitalEnumResource05 { get; set; }
        public uint VitalEnumResource06 { get; set; }
        public uint VitalEnumResource07 { get; set; }
        public uint LocalizedTextIdResourceAlert00 { get; set; }
        public uint LocalizedTextIdResourceAlert01 { get; set; }
        public uint LocalizedTextIdResourceAlert02 { get; set; }
        public uint LocalizedTextIdResourceAlert03 { get; set; }
        public uint LocalizedTextIdResourceAlert04 { get; set; }
        public uint LocalizedTextIdResourceAlert05 { get; set; }
        public uint LocalizedTextIdResourceAlert06 { get; set; }
        public uint LocalizedTextIdResourceAlert07 { get; set; }
        public uint AttributeMilestoneGroupId00 { get; set; }
        public uint AttributeMilestoneGroupId01 { get; set; }
        public uint AttributeMilestoneGroupId02 { get; set; }
        public uint AttributeMilestoneGroupId03 { get; set; }
        public uint AttributeMilestoneGroupId04 { get; set; }
        public uint AttributeMilestoneGroupId05 { get; set; }
        public uint ClassSecondaryStatBonusId00 { get; set; }
        public uint ClassSecondaryStatBonusId01 { get; set; }
        public uint ClassSecondaryStatBonusId02 { get; set; }
        public uint ClassSecondaryStatBonusId03 { get; set; }
        public uint ClassSecondaryStatBonusId04 { get; set; }
        public uint ClassSecondaryStatBonusId05 { get; set; }
        public uint AttributeMiniMilestoneGroupId00 { get; set; }
        public uint AttributeMiniMilestoneGroupId01 { get; set; }
        public uint AttributeMiniMilestoneGroupId02 { get; set; }
        public uint AttributeMiniMilestoneGroupId03 { get; set; }
        public uint AttributeMiniMilestoneGroupId04 { get; set; }
        public uint AttributeMiniMilestoneGroupId05 { get; set; }
        public uint AttributeMilestoneMaxTiers00 { get; set; }
        public uint AttributeMilestoneMaxTiers01 { get; set; }
        public uint AttributeMilestoneMaxTiers02 { get; set; }
        public uint AttributeMilestoneMaxTiers03 { get; set; }
        public uint AttributeMilestoneMaxTiers04 { get; set; }
        public uint AttributeMilestoneMaxTiers05 { get; set; }
    }
}

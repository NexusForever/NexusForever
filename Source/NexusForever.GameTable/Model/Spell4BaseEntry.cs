namespace NexusForever.GameTable.Model
{
    public class Spell4BaseEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint Spell4HitResultId { get; set; }
        public uint Spell4TargetMechanicId { get; set; }
        public uint Spell4TargetAngleId { get; set; }
        public uint Spell4PrerequisiteId { get; set; }
        public uint Spell4ValidTargetId { get; set; }
        public uint TargetGroupIdCastGroup { get; set; }
        public uint Creature2IdPositionalAoe { get; set; }
        public float ParameterAEAngle { get; set; }
        public float ParameterAEMaxAngle { get; set; }
        public float ParameterAEDistance { get; set; }
        public float ParameterAEMaxDistance { get; set; }
        public uint TargetGroupIdAoeGroup { get; set; }
        public uint Spell4BaseIdPrerequisiteSpell { get; set; }
        public uint WorldZoneIdZoneRequired { get; set; }
        public uint Spell4SpellTypesIdSpellType { get; set; }
        public string Icon { get; set; }
        public uint CastMethod { get; set; }
        public uint School { get; set; }
        public uint SpellClass { get; set; }
        public uint WeaponSlot { get; set; }
        public uint CastBarType { get; set; }
        public float MechanicAggressionMagnitude { get; set; }
        public float MechanicDominationMagnitude { get; set; }
        public uint ModelSequencePriorityCaster { get; set; }
        public uint ModelSequencePriorityTarget { get; set; }
        public uint ClassIdPlayer { get; set; }
        public uint ClientSideInteractionId { get; set; }
        public uint TargetingFlags { get; set; }
        public uint TelegraphFlagsEnum { get; set; }
        public uint LocalizedTextIdLASTierPoint { get; set; }
        public float LasTierPointTooltipData00 { get; set; }
        public float LasTierPointTooltipData01 { get; set; }
        public float LasTierPointTooltipData02 { get; set; }
        public float LasTierPointTooltipData03 { get; set; }
        public float LasTierPointTooltipData04 { get; set; }
        public float LasTierPointTooltipData05 { get; set; }
        public float LasTierPointTooltipData06 { get; set; }
        public float LasTierPointTooltipData07 { get; set; }
        public float LasTierPointTooltipData08 { get; set; }
        public float LasTierPointTooltipData09 { get; set; }
    }
}

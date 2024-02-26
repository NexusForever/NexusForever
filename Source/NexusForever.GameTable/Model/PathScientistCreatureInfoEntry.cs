namespace NexusForever.GameTable.Model
{
    public class PathScientistCreatureInfoEntry
    {
        public uint Id { get; set; }
        public uint ScientistCreatureFlags { get; set; }
        public string DisplayIcon { get; set; }
        public uint PrerequisiteIdScan { get; set; }
        public uint PrerequisiteIdRawScan { get; set; }
        public uint PrerequisiteIdScanCreature { get; set; }
        public uint PrerequisiteIdRawScanCreature { get; set; }
        public uint Spell4IdBuff00 { get; set; }
        public uint Spell4IdBuff01 { get; set; }
        public uint Spell4IdBuff02 { get; set; }
        public uint Spell4IdBuff03 { get; set; }
        public uint ChecklistCount { get; set; }
        public uint ScientistCreatureTypeEnum { get; set; }
        public uint LootId { get; set; }
    }
}

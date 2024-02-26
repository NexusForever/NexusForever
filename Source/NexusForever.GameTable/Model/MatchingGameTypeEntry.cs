namespace NexusForever.GameTable.Model
{
    public class MatchingGameTypeEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint LocalizedTextIdDescription { get; set; }
        public uint MatchTypeEnum { get; set; }
        public uint MatchingGameTypeEnumFlags { get; set; }
        public uint TeamSize { get; set; }
        public uint MinLevel { get; set; }
        public uint MaxLevel { get; set; }
        public uint PreparationTimeMS { get; set; }
        public uint MatchTimeMS { get; set; }
        public uint MatchingRulesEnum { get; set; }
        public uint MatchingRulesData00 { get; set; }
        public uint MatchingRulesData01 { get; set; }
        public uint TargetItemLevel { get; set; }
    }
}

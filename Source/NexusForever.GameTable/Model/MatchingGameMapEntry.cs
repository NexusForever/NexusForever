namespace NexusForever.GameTable.Model
{
    public class MatchingGameMapEntry
    {
        public uint Id { get; set; }
        public uint MatchingGameMapEnumFlags { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint LocalizedTextIdDescription { get; set; }
        public uint MatchingGameTypeId { get; set; }
        public uint WorldId { get; set; }
        public uint RecommendedItemLevel { get; set; }
        public uint AchievementCategoryId { get; set; }
        public uint PrerequisiteId { get; set; }
    }
}

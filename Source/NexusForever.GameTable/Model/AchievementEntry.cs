namespace NexusForever.GameTable.Model
{
    public class AchievementEntry
    {
        public uint Id { get; set; }
        public uint AchievementTypeId { get; set; }
        public uint AchievementCategoryId { get; set; }
        public uint Flags { get; set; }
        public uint WorldZoneId { get; set; }
        public uint LocalizedTextIdTitle { get; set; }
        public uint LocalizedTextIdDesc { get; set; }
        public uint LocalizedTextIdProgress { get; set; }
        public float PercCompletionToShow { get; set; }
        public uint ObjectId { get; set; }
        public uint ObjectIdAlt { get; set; }
        public uint Value { get; set; }
        public uint CharacterTitleId { get; set; }
        public uint PrerequisiteId { get; set; }
        public uint PrerequisiteIdServer { get; set; }
        public uint PrerequisiteIdObjective { get; set; }
        public uint PrerequisiteIdObjectiveAlt { get; set; }
        public uint AchievementIdParentTier { get; set; }
        public uint OrderIndex { get; set; }
        public uint AchievementGroupId { get; set; }
        public uint AchievementSubGroupId { get; set; }
        public uint AchievementPointEnum { get; set; }
        public string SteamAchievementName { get; set; }
    }
}

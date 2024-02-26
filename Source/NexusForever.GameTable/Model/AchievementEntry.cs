namespace NexusForever.GameTable.Model
{
    public class AchievementEntry
    {
        public uint Id { get { get; set; } set { get; set; } }
        public uint AchievementTypeId { get { get; set; } set { get; set; } }
        public uint AchievementCategoryId { get { get; set; } set { get; set; } }
        public uint Flags { get { get; set; } set { get; set; } }
        public uint WorldZoneId { get { get; set; } set { get; set; } }
        public uint LocalizedTextIdTitle { get { get; set; } set { get; set; } }
        public uint LocalizedTextIdDesc { get { get; set; } set { get; set; } }
        public uint LocalizedTextIdProgress { get { get; set; } set { get; set; } }
        public float PercCompletionToShow { get { get; set; } set { get; set; } }
        public uint ObjectId { get { get; set; } set { get; set; } }
        public uint ObjectIdAlt { get { get; set; } set { get; set; } }
        public uint Value { get { get; set; } set { get; set; } }
        public uint CharacterTitleId { get { get; set; } set { get; set; } }
        public uint PrerequisiteId { get { get; set; } set { get; set; } }
        public uint PrerequisiteIdServer { get { get; set; } set { get; set; } }
        public uint PrerequisiteIdObjective { get { get; set; } set { get; set; } }
        public uint PrerequisiteIdObjectiveAlt { get { get; set; } set { get; set; } }
        public uint AchievementIdParentTier { get { get; set; } set { get; set; } }
        public uint OrderIndex { get { get; set; } set { get; set; } }
        public uint AchievementGroupId { get { get; set; } set { get; set; } }
        public uint AchievementSubGroupId { get { get; set; } set { get; set; } }
        public uint AchievementPointEnum { get { get; set; } set { get; set; } }
        public string SteamAchievementName { get { get; set; } set { get; set; } }
    }
}

namespace NexusForever.Shared.GameTable.Model
{
    public class AchievementEntry
    {
        public uint Id;
        public uint AchievementTypeId;
        public uint AchievementCategoryId;
        public uint Flags;
        public uint WorldZoneId;
        public uint LocalizedTextIdTitle;
        public uint LocalizedTextIdDesc;
        public uint LocalizedTextIdProgress;
        public float PercCompletionToShow;
        public uint ObjectId;
        public uint ObjectIdAlt;
        public uint Value;
        public uint CharacterTitleId;
        public uint PrerequisiteId;
        public uint PrerequisiteIdServer;
        public uint PrerequisiteIdObjective;
        public uint PrerequisiteIdObjectiveAlt;
        public uint AchievementIdParentTier;
        public uint OrderIndex;
        public uint AchievementGroupId;
        public uint AchievementSubGroupId;
        public uint AchievementPointEnum;
        public string SteamAchievementName;
    }
}

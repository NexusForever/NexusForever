namespace NexusForever.GameTable.Model
{
    public class AchievementChecklistEntry
    {
        public uint Id { get; set; }
        public uint AchievementId { get; set; }
        public uint Bit { get; set; }
        public uint ObjectId { get; set; }
        public uint ObjectIdAlt { get; set; }
        public uint PrerequisiteId { get; set; }
        public uint PrerequisiteIdAlt { get; set; }
    }
}

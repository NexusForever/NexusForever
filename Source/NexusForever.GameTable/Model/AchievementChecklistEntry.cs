namespace NexusForever.GameTable.Model
{
    public class AchievementChecklistEntry
    {
        public uint Id { get { get; set; } set { get; set; } }
        public uint AchievementId { get { get; set; } set { get; set; } }
        public uint Bit { get { get; set; } set { get; set; } }
        public uint ObjectId { get { get; set; } set { get; set; } }
        public uint ObjectIdAlt { get { get; set; } set { get; set; } }
        public uint PrerequisiteId { get { get; set; } set { get; set; } }
        public uint PrerequisiteIdAlt { get { get; set; } set { get; set; } }
    }
}

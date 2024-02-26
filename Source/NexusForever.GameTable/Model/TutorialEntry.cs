namespace NexusForever.GameTable.Model
{
    public class TutorialEntry
    {
        public uint Id { get; set; }
        public uint Flags { get; set; }
        public uint TutorialCategoryEnum { get; set; }
        public uint LocalizedTextIdContextualPopup { get; set; }
        public uint TutorialAnchorId { get; set; }
        public uint RequiredLevel { get; set; }
        public uint PrerequisiteId { get; set; }
    }
}

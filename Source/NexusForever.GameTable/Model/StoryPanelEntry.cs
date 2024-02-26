namespace NexusForever.GameTable.Model
{
    public class StoryPanelEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextIdBody { get; set; }
        public uint SoundEventId { get; set; }
        public uint WindowTypeId { get; set; }
        public uint DurationMS { get; set; }
        public uint PrerequisiteId { get; set; }
        public uint StoryPanelStyleEnum { get; set; }
    }
}

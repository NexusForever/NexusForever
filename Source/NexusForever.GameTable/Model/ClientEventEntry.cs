namespace NexusForever.GameTable.Model
{
    public class ClientEventEntry
    {
        public uint Id { get; set; }
        public string Description { get; set; }
        public uint WorldId { get; set; }
        public uint EventTypeEnum { get; set; }
        public uint EventData { get; set; }
        public uint PrerequisiteId { get; set; }
        public uint Priority { get; set; }
        public uint DelayMS { get; set; }
        public uint ClientEventActionId { get; set; }
    }
}

namespace NexusForever.GameTable.Model
{
    public class PublicEventCustomStatEntry
    {
        public uint Id { get; set; }
        public uint PublicEventTypeEnum { get; set; }
        public uint PublicEventId { get; set; }
        public uint StatIndex { get; set; }
        public uint LocalizedTextIdStatName { get; set; }
        public string IconPath { get; set; }
    }
}

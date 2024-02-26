namespace NexusForever.GameTable.Model
{
    public class InstancePortalEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint MinLevel { get; set; }
        public uint MaxLevel { get; set; }
        public uint ExpectedCompletionTime { get; set; }
        public uint InstancePortalTypeEnum { get; set; }
    }
}

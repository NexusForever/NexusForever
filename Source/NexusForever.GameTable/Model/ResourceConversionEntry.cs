namespace NexusForever.GameTable.Model
{
    public class ResourceConversionEntry
    {
        public uint Id { get; set; }
        public uint ResourceConversionTypeEnum { get; set; }
        public uint SourceId { get; set; }
        public uint SourceCount { get; set; }
        public uint TargetId { get; set; }
        public uint TargetCount { get; set; }
        public uint SurchargeId { get; set; }
        public uint SurchargeCount { get; set; }
        public uint Flags { get; set; }
    }
}

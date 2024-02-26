namespace NexusForever.GameTable.Model
{
    public class WordFilterEntry
    {
        public uint Id { get; set; }
        public string Filter { get; set; }
        public uint UserTextFilterClassEnum { get; set; }
        public uint DeploymentRegionEnum { get; set; }
        public uint LanguageId { get; set; }
        public uint WordFilterTypeEnum { get; set; }
    }
}

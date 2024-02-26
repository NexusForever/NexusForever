namespace NexusForever.GameTable.Model
{
    public class ArchiveLinkEntry
    {
        public uint Id { get; set; }
        public uint ArchiveArticleIdParent { get; set; }
        public uint ArchiveArticleIdChild { get; set; }
        public uint ArchiveLinkFlags { get; set; }
    }
}

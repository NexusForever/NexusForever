namespace NexusForever.GameTable.Model
{
    public class PathExplorerScavengerClueEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextIdClue { get; set; }
        public uint ExplorerScavengerClueTypeEnum { get; set; }
        public uint Creature2Id { get; set; }
        public uint TargetGroupId { get; set; }
        public float ActiveRadius { get; set; }
        public uint WorldLocation2IdMiniMap { get; set; }
    }
}

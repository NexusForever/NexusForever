namespace NexusForever.GameTable.Model
{
    public class TargetGroupEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextIdDisplayString { get; set; }
        public uint Type { get; set; }
        [GameTableFieldArray(7u)]
        public uint[] DataEntries { get; set; }
    }
}

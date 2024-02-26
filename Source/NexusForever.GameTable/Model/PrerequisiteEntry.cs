namespace NexusForever.GameTable.Model
{
    public class PrerequisiteEntry
    {
        public uint Id { get; set; }
        public uint Flags { get; set; }
        [GameTableFieldArray(3u)]
        public uint[] PrerequisiteTypeId { get; set; }
        [GameTableFieldArray(3u)]
        public uint[] PrerequisiteComparisonId { get; set; }
        [GameTableFieldArray(3u)]
        public uint[] ObjectId { get; set; }
        [GameTableFieldArray(3u)]
        public uint[] Value { get; set; }
        public uint LocalizedTextIdFailure { get; set; }
    }
}

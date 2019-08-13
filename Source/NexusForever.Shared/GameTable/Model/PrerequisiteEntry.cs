namespace NexusForever.Shared.GameTable.Model
{
    public class PrerequisiteEntry
    {
        public uint Id;
        public uint Flags;
        [GameTableFieldArray(3u)]
        public uint[] PrerequisiteTypeId;
        [GameTableFieldArray(3u)]
        public uint[] PrerequisiteComparisonId;
        [GameTableFieldArray(3u)]
        public uint[] ObjectId;
        [GameTableFieldArray(3u)]
        public uint[] Value;
        public uint LocalizedTextIdFailure;
    }
}

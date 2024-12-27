using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.GameTable.Model
{
    public class PrerequisiteEntry
    {
        public uint Id;
        public EvaluationMode Flags;
        [GameTableFieldArray(3u)]
        public PrerequisiteType[] PrerequisiteTypeId;
        [GameTableFieldArray(3u)]
        public PrerequisiteComparison[] PrerequisiteComparisonId;
        [GameTableFieldArray(3u)]
        public uint[] ObjectId;
        [GameTableFieldArray(3u)]
        public uint[] Value;
        public uint LocalizedTextIdFailure;
    }
}

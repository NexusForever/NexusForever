namespace NexusForever.Shared.GameTable.Model
{
    public class TargetGroupEntry
    {
        public uint Id;
        public uint LocalizedTextIdDisplayString;
        public uint Type;
        [GameTableFieldArray(7u)]
        public uint[] DataEntries;
    }
}

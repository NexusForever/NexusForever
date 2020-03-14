namespace NexusForever.Shared.GameTable.Model
{
    public class WorldSocketEntry
    {
        public uint Id;
        public uint WorldId;
        [GameTableFieldArray(4u)]
        public uint[] BoundIds;
        public uint AverageHeight;
    }
}

namespace NexusForever.GameTable.Model
{
    public class WorldSocketEntry
    {
        public uint Id { get; set; }
        public uint WorldId { get; set; }
        [GameTableFieldArray(4u)]
        public uint[] BoundIds { get; set; }
        public uint AverageHeight { get; set; }
    }
}

namespace NexusForever.GameTable.Model
{
    public class CreatureLevelEntry
    {
        public uint Id { get; set; }
        [GameTableFieldArray(100)]
        public float[] UnitPropertyValue { get; set; }
    }
}

namespace NexusForever.GameTable.Model
{
    public class CreatureLevelEntry
    {
        public uint Id;
        [GameTableFieldArray(100)]
        public float[] UnitPropertyValue;
    }
}

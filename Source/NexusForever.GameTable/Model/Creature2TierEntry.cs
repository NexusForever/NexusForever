namespace NexusForever.GameTable.Model
{
    public class Creature2TierEntry
    {
        public uint Id { get; set; }
        [GameTableFieldArray(200)]
        public float[] UnitPropertyMultiplier { get; set; }
    }
}

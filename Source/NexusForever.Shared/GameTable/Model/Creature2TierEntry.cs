namespace NexusForever.Shared.GameTable.Model
{
    public class Creature2TierEntry
    {
        public uint Id;
        [GameTableFieldArray(200)]
        public float[] UnitPropertyMultiplier;
    }
}

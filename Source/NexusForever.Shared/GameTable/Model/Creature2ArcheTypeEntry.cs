namespace NexusForever.Shared.GameTable.Model
{
    public class Creature2ArcheTypeEntry
    {
        public uint Id;
        public string Icon;
        [GameTableFieldArray(200)]
        public float[] UnitPropertyMultiplier;
    }
}

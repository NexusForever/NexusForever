namespace NexusForever.GameTable.Model
{
    public class Creature2ArcheTypeEntry
    {
        public uint Id { get; set; }
        public string Icon { get; set; }
        [GameTableFieldArray(200)]
        public float[] UnitPropertyMultiplier { get; set; }
    }
}

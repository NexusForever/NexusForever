namespace NexusForever.GameTable.Model
{
    public class Spell4ModificationEntry
    {
        public uint Id { get; set; }
        public uint Spell4EffectsId { get; set; }
        public uint ModificationParameterEnum { get; set; }
        public uint Priority { get; set; }
        public uint ModificationTypeEnum { get; set; }
        public float Data00 { get; set; }
        public float Data01 { get; set; }
        public float Data02 { get; set; }
        public float Data03 { get; set; }
    }
}

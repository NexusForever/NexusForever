namespace NexusForever.GameTable.Model
{
    public class DyeColorRampEntry
    {
        public uint Id { get; set; }
        public uint Flags { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint RampIndex { get; set; }
        public float CostMultiplier { get; set; }
        public uint ComponentMapEnum { get; set; }
        public uint PrerequisiteId { get; set; }
    }
}

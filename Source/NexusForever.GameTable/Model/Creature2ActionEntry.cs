namespace NexusForever.GameTable.Model
{
    public class Creature2ActionEntry
    {
        public uint Id { get; set; }
        public string Description { get; set; }
        public uint CreatureActionSetId { get; set; }
        public uint State { get; set; }
        public uint Event { get; set; }
        public uint OrderIndex { get; set; }
        public uint DelayMS { get; set; }
        public uint Action { get; set; }
        public uint ActionData00 { get; set; }
        public uint ActionData01 { get; set; }
        public uint VisualEffectId { get; set; }
        public uint PrerequisiteId { get; set; }
    }
}

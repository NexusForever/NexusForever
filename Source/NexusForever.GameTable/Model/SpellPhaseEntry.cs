namespace NexusForever.GameTable.Model
{
    public class SpellPhaseEntry
    {
        public uint Id { get; set; }
        public uint PhaseDelay { get; set; }
        public uint PhaseFlags { get; set; }
        public uint OrderIndex { get; set; }
        public uint Spell4IdOwner { get; set; }
    }
}

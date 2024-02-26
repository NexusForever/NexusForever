namespace NexusForever.GameTable.Model
{
    public class Spell4AoeTargetConstraintsEntry
    {
        public uint Id { get; set; }
        public float Angle { get; set; }
        public uint TargetCount { get; set; }
        public float MinRange { get; set; }
        public float MaxRange { get; set; }
        public uint TargetSelection { get; set; }
    }
}

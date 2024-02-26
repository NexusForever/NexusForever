namespace NexusForever.GameTable.Model
{
    public class WorldLocation2Entry
    {
        public uint Id { get; set; }
        public float Radius { get; set; }
        public float MaxVerticalDistance { get; set; }
        public float Position0 { get; set; }
        public float Position1 { get; set; }
        public float Position2 { get; set; }
        public float Facing0 { get; set; }
        public float Facing1 { get; set; }
        public float Facing2 { get; set; }
        public float Facing3 { get; set; }
        public uint WorldId { get; set; }
        public uint WorldZoneId { get; set; }
        public uint Phases { get; set; }
    }
}

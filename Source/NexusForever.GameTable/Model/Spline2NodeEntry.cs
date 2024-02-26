namespace NexusForever.GameTable.Model
{
    public class Spline2NodeEntry
    {
        public uint Id { get; set; }
        public uint SplineId { get; set; }
        public uint Ordinal { get; set; }
        public float Position0 { get; set; }
        public float Position1 { get; set; }
        public float Position2 { get; set; }
        public float Facing0 { get; set; }
        public float Facing1 { get; set; }
        public float Facing2 { get; set; }
        public float Facing3 { get; set; }
        public uint EventId { get; set; }
        public float FrameTime { get; set; }
        public float Delay { get; set; }
        public float Fovy { get; set; }
    }
}

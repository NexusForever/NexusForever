namespace NexusForever.GameTable.Model
{
    public class WorldWaterWakeEntry
    {
        public uint Id { get; set; }
        public uint Flags { get; set; }
        public string ColorTexture { get; set; }
        public string NormalTexture { get; set; }
        public string DistortionTexture { get; set; }
        public uint DurationMin { get; set; }
        public uint DurationMax { get; set; }
        public float ScaleStart { get; set; }
        public float ScaleEnd { get; set; }
        public float AlphaStart { get; set; }
        public float AlphaEnd { get; set; }
        public float DistortionWeight { get; set; }
        public float DistortionScaleStart { get; set; }
        public float DistortionScaleEnd { get; set; }
        public float DistortionSpeedU { get; set; }
        public float DistortionSpeedV { get; set; }
        public float PositionOffsetX { get; set; }
        public float PositionOffsetY { get; set; }
    }
}

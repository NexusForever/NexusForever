namespace NexusForever.GameTable.Model
{
    public class WindEntry
    {
        public uint Id { get; set; }
        public uint Type { get; set; }
        public uint Duration { get; set; }
        public float RadiusEnd { get; set; }
        public float Direction { get; set; }
        public float DirectionDelta { get; set; }
        public float BlendIn { get; set; }
        public float BlendOut { get; set; }
        public float Speed { get; set; }
        public float Sine2DMagnitudeMin { get; set; }
        public float Sine2DMagnitudeMax { get; set; }
        public float Sine2DFrequency { get; set; }
        public float Sine2DOffsetAngle { get; set; }
        public uint LocalRadial { get; set; }
        public float LocalMagnitude { get; set; }
    }
}

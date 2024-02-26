namespace NexusForever.GameTable.Model
{
    public class WorldWaterLayerEntry
    {
        public uint Id { get; set; }
        public string Description { get; set; }
        public string RippleColorTex { get; set; }
        public string RippleNormalTex { get; set; }
        public float Scale { get; set; }
        public float Rotation { get; set; }
        public float Speed { get; set; }
        public float OscFrequency { get; set; }
        public float OscMagnitude { get; set; }
        public float OscRotation { get; set; }
        public float OscPhase { get; set; }
        public float OscMinLayerWeight { get; set; }
        public float OscMaxLayerWeight { get; set; }
        public float OscLayerWeightPhase { get; set; }
        public float MaterialBlend { get; set; }
    }
}

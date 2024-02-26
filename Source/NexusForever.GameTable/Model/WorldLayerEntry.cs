namespace NexusForever.GameTable.Model
{
    public class WorldLayerEntry
    {
        public uint Id { get; set; }
        public string Description { get; set; }
        public float HeightScale { get; set; }
        public float HeightOffset { get; set; }
        public float ParallaxScale { get; set; }
        public float ParallaxOffset { get; set; }
        public float MetersPerTextureTile { get; set; }
        public string ColorMapPath { get; set; }
        public string NormalMapPath { get; set; }
        public uint AverageColor { get; set; }
        public uint Projection { get; set; }
        public uint MaterialType { get; set; }
        public uint WorldClutterId00 { get; set; }
        public uint WorldClutterId01 { get; set; }
        public uint WorldClutterId02 { get; set; }
        public uint WorldClutterId03 { get; set; }
        public float SpecularPower { get; set; }
        public uint EmissiveGlow { get; set; }
        public float ScrollSpeed00 { get; set; }
        public float ScrollSpeed01 { get; set; }
    }
}

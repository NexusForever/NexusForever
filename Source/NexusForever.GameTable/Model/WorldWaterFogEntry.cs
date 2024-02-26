namespace NexusForever.GameTable.Model
{
    public class WorldWaterFogEntry
    {
        public uint Id { get; set; }
        public float FogStart { get; set; }
        public float FogEnd { get; set; }
        public float FogStartUW { get; set; }
        public float FogEndUW { get; set; }
        public float ModStart { get; set; }
        public float ModEnd { get; set; }
        public float ModStartUW { get; set; }
        public float ModEndUW { get; set; }
        public uint SkyColorIndex { get; set; }
    }
}

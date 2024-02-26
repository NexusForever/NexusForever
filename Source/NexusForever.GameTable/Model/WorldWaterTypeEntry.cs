namespace NexusForever.GameTable.Model
{
    public class WorldWaterTypeEntry
    {
        public uint Id { get; set; }
        public uint WorldWaterFogId { get; set; }
        public uint SurfaceType { get; set; }
        public string ParticleFile { get; set; }
        public uint SoundDirectionalAmbienceId { get; set; }
    }
}

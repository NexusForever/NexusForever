namespace NexusForever.GameTable.Model
{
    public class WorldEntry
    {
        public uint Id { get; set; }
        public string AssetPath { get; set; }
        public uint Flags { get; set; }
        public uint Type { get; set; }
        public string ScreenPath { get; set; }
        public string ScreenModelPath { get; set; }
        public uint ChunkBounds00 { get; set; }
        public uint ChunkBounds01 { get; set; }
        public uint ChunkBounds02 { get; set; }
        public uint ChunkBounds03 { get; set; }
        public uint PlugAverageHeight { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint MinItemLevel { get; set; }
        public uint MaxItemLevel { get; set; }
        public uint PrimeLevelOffset { get; set; }
        public uint PrimeLevelMax { get; set; }
        public uint VeteranTierScalingType { get; set; }
        public uint HeroismMenaceLevel { get; set; }
        public uint RewardRotationContentId { get; set; }
    }
}

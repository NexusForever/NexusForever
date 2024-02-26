namespace NexusForever.GameTable.Model
{
    public class MapContinentEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public string AssetPath { get; set; }
        public string ImagePath { get; set; }
        public uint ImageWidth { get; set; }
        public uint ImageHeight { get; set; }
        public uint ImageOffsetX { get; set; }
        public uint ImageOffsetY { get; set; }
        public uint HexMinX { get; set; }
        public uint HexMinY { get; set; }
        public uint HexLimX { get; set; }
        public uint HexLimY { get; set; }
        public uint Flags { get; set; }
    }
}

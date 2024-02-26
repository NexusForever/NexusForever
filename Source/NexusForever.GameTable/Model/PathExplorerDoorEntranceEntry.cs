namespace NexusForever.GameTable.Model
{
    public class PathExplorerDoorEntranceEntry
    {
        public uint Id { get; set; }
        public uint PathExplorerDoorTypeEnumSurface { get; set; }
        public uint PathExplorerDoorTypeEnumMicro { get; set; }
        public uint Creature2IdSurface { get; set; }
        public uint Creature2IdMicro { get; set; }
        public uint PathExplorerDoorId { get; set; }
        public uint WorldLocation2IdSurfaceRevealed { get; set; }
    }
}

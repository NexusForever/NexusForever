namespace NexusForever.GameTable.Model
{
    public class MapZoneEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint MapContinentId { get; set; }
        public string Folder { get; set; }
        public uint HexMinX { get; set; }
        public uint HexMinY { get; set; }
        public uint HexLimX { get; set; }
        public uint HexLimY { get; set; }
        public uint Version { get; set; }
        public uint MapZoneIdParent { get; set; }
        public uint WorldZoneId { get; set; }
        public uint Flags { get; set; }
        public uint PrerequisiteIdVisibility { get; set; }
        public uint RewardTrackId { get; set; }
    }
}

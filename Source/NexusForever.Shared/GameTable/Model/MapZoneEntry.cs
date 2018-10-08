namespace NexusForever.Shared.GameTable.Model
{
    public class MapZoneEntry
    {
        public uint Id;
        public uint LocalizedTextIdName;
        public uint MapContinentId;
        public string Folder;
        public uint HexMinX;
        public uint HexMinY;
        public uint HexLimX;
        public uint HexLimY;
        public uint Version;
        public uint MapZoneIdParent;
        public uint WorldZoneId;
        public uint Flags;
        public uint PrerequisiteIdVisibility;
        public uint RewardTrackId;
    }
}

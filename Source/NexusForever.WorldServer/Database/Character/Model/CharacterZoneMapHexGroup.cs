using System;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterZoneMapHexGroup
    {
        public ulong Id { get; set; }
        public ushort ZoneMap { get; set; }
        public ushort HexGroup { get; set; }

        public virtual Character IdNavigation { get; set; }
    }
}

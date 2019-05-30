using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterZonemapHexgroup
    {
        public ulong Id { get; set; }
        public ushort ZoneMap { get; set; }
        public ushort HexGroup { get; set; }

        public virtual Character IdNavigation { get; set; }
    }
}

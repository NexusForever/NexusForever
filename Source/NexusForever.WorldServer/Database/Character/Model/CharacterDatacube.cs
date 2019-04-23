using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterDatacube
    {
        public ulong Id { get; set; }
        public byte Type { get; set; }
        public ushort DatacubeId { get; set; }
        public uint Progress { get; set; }

        public Character IdNavigation { get; set; }
    }
}

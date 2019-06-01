using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterDatacube
    {
        public ulong Id { get; set; }
        public byte Type { get; set; }
        public ushort Datacube { get; set; }
        public uint Progress { get; set; }

        public virtual Character IdNavigation { get; set; }
    }
}

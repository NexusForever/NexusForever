using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterActionSetAmp
    {
        public ulong Id { get; set; }
        public byte SpecIndex { get; set; }
        public ushort AmpId { get; set; }

        public virtual Character IdNavigation { get; set; }
    }
}

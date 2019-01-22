using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterAMP
    {
        public ulong Id { get; set; }
        public byte SpecIndex { get; set; }
        public ushort AMPId { get; set; }

        public Character IdNavigation { get; set; }
    }
}

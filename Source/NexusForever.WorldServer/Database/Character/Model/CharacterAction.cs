using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterAction
    {
        public ulong Id { get; set; }
        public byte SpecIndex { get; set; }
        public ushort Location { get; set; }
        public uint Action { get; set; }
        public byte TierIndex { get; set; }

        public Character IdNavigation { get; set; }
    }
}

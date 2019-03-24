using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterActionSetShortcut
    {
        public ulong Id { get; set; }
        public byte SpecIndex { get; set; }
        public ushort Location { get; set; }
        public byte ShortcutType { get; set; }
        public uint ObjectId { get; set; }
        public byte Tier { get; set; }

        public virtual Character IdNavigation { get; set; }
    }
}

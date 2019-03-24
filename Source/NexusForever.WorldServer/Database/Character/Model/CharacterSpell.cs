using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterSpell
    {
        public ulong Id { get; set; }
        public uint Spell4BaseId { get; set; }
        public byte Tier { get; set; }

        public virtual Character IdNavigation { get; set; }
    }
}

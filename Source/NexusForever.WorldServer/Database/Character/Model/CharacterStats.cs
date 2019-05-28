using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterStats
    {
        public ulong Id { get; set; }
        public byte Stat { get; set; }
        public float Value { get; set; }

        public virtual Character IdNavigation { get; set; }
    }
}

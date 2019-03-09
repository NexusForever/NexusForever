using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterAppearance
    {
        public ulong Id { get; set; }
        public byte Slot { get; set; }
        public ushort DisplayId { get; set; }

        public virtual Character IdNavigation { get; set; }
    }
}

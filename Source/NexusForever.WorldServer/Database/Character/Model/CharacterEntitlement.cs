using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterEntitlement
    {
        public ulong Id { get; set; }
        public byte EntitlementId { get; set; }
        public uint Amount { get; set; }

        public virtual Character IdNavigation { get; set; }
    }
}

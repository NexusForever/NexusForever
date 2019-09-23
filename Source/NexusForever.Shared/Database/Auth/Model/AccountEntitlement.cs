using System;
using System.Collections.Generic;

namespace NexusForever.Shared.Database.Auth.Model
{
    public partial class AccountEntitlement
    {
        public uint Id { get; set; }
        public byte EntitlementId { get; set; }
        public uint Amount { get; set; }

        public virtual Account IdNavigation { get; set; }
    }
}

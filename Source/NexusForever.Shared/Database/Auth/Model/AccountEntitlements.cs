using System;
using System.Collections.Generic;

namespace NexusForever.Shared.Database.Auth.Model
{
    public partial class AccountEntitlements
    {
        public uint Id { get; set; }
        public uint EntitlementId { get; set; }
        public uint Amount { get; set; }

        public virtual Account IdNavigation { get; set; }
    }
}

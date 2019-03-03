using System;
using System.Collections.Generic;

namespace NexusForever.Shared.Database.Auth.Model
{
    public partial class AccountGenericUnlock
    {
        public uint Id { get; set; }
        public uint Entry { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual Account IdNavigation { get; set; }
    }
}

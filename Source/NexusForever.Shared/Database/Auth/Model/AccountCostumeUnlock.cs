using System;
using System.Collections.Generic;

namespace NexusForever.Shared.Database.Auth.Model
{
    public partial class AccountCostumeUnlock
    {
        public uint Id { get; set; }
        public uint ItemId { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual Account IdNavigation { get; set; }
    }
}

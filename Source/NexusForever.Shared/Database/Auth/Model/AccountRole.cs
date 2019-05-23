using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.Shared.Database.Auth.Model
{
    public partial class AccountRole
    {
        public uint AccountId { get; set; }
        public ulong RoleId { get; set; }

        public virtual Account IdNavigation { get; set; }
    }
}

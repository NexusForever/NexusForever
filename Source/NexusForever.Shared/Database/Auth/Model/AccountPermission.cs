using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.Shared.Database.Auth.Model
{
    public partial class AccountPermission
    {
        public uint Id { get; set; }
        public long PermissionId { get; set; }

        public virtual Account IdNavigation { get; set; }
    }
}

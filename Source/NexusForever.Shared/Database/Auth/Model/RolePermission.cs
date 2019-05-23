using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.Shared.Database.Auth.Model
{
    public partial class RolePermission
    {
        public ulong Id { get; set; }
        public long PermissionId { get; set; }

        public virtual Role IdNavigation { get; set; }
    }
}

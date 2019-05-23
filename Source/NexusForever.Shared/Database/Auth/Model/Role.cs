using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.Shared.Database.Auth.Model
{
    public partial class Role
    {
        public Role()
        {
            RolePermission = new HashSet<RolePermission>();
        }

        public ulong Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<RolePermission> RolePermission { get; set; }
    }
}

using System.Collections.Generic;

namespace NexusForever.Database.Auth.Model
{
    public class PermissionModel
    {
        public uint Id { get; set; }
        public string Name { get; set; }

        public ICollection<RolePermissionModel> RolePermission { get; set; } = new HashSet<RolePermissionModel>();
        public ICollection<AccountPermissionModel> AccountPermission { get; set; } = new HashSet<AccountPermissionModel>();
    }
}

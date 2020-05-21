using System.Collections.Generic;

namespace NexusForever.Database.Auth.Model
{
    public class RoleModel
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public uint Flags { get; set; }

        public ICollection<RolePermissionModel> RolePermission { get; set; } = new HashSet<RolePermissionModel>();
        public ICollection<AccountRoleModel> AccountRole { get; set; } = new HashSet<AccountRoleModel>();
    }
}

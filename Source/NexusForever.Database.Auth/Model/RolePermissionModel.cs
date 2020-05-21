namespace NexusForever.Database.Auth.Model
{
    public class RolePermissionModel
    {
        public uint Id { get; set; }
        public uint PermissionId { get; set; }

        public RoleModel Role { get; set; }
        public PermissionModel Permission { get; set; }
    }
}

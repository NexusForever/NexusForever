namespace NexusForever.Database.Auth.Model
{
    public class AccountPermissionModel
    {
        public uint Id { get; set; }
        public uint PermissionId { get; set; }

        public AccountModel Account { get; set; }
        public PermissionModel Permission { get; set; }
    }
}

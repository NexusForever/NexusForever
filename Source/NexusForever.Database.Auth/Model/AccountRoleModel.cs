namespace NexusForever.Database.Auth.Model
{
    public class AccountRoleModel
    {
        public uint Id { get; set; }
        public uint RoleId { get; set; }

        public AccountModel Account { get; set; }
        public RoleModel Role { get; set; }
    }
}

namespace NexusForever.Database.Auth.Model
{
    public class AccountEntitlementModel
    {
        public uint Id { get; set; }
        public byte EntitlementId { get; set; }
        public uint Amount { get; set; }

        public AccountModel Account { get; set; }
    }
}

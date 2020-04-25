namespace NexusForever.Database.Auth.Model
{
    public class AccountItemModel
    {
        public ulong Id { get; set; }
        public uint AccountId { get; set; }
        public uint ItemId { get; set; }

        public AccountModel Account { get; set; }
    }
}

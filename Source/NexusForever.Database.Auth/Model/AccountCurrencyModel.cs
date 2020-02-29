namespace NexusForever.Database.Auth.Model
{
    public class AccountCurrencyModel
    {
        public uint Id { get; set; }
        public byte CurrencyId { get; set; }
        public ulong Amount { get; set; }

        public AccountModel Account { get; set; }
    }
}

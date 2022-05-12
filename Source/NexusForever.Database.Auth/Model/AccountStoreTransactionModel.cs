using System;

namespace NexusForever.Database.Auth.Model
{
    public class AccountStoreTransactionModel
    {
        public uint Id { get; set; }
        public ulong TransactionId { get; set; }
        public string Name { get; set; }
        public ushort CurrencyType { get; set; }
        public float Cost { get; set; }
        public DateTime TransactionDateTime { get; set; }

        public AccountModel Account { get; set; }
    }
}

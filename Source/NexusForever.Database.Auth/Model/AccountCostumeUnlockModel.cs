using System;

namespace NexusForever.Database.Auth.Model
{
    public class AccountCostumeUnlockModel
    {
        public uint Id { get; set; }
        public uint ItemId { get; set; }
        public DateTime Timestamp { get; set; }

        public AccountModel Account { get; set; }
    }
}

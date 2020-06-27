using System;

namespace NexusForever.Database.Auth.Model
{
    public class AccountGenericUnlockModel
    {
        public uint Id { get; set; }
        public uint Entry { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual AccountModel Account { get; set; }
    }
}

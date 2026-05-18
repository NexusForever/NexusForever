using System;

namespace NexusForever.Database.Auth.Model
{
    public class AccountSuspensionModel
    {
        public uint Id { get; set; }
        public uint BanId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Reason { get; set; }

        public AccountModel Account { get; set; }
    }
}

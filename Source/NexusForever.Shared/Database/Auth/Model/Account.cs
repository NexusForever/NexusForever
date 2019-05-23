using System;
using System.Collections.Generic;

namespace NexusForever.Shared.Database.Auth.Model
{
    public partial class Account
    {
        public Account()
        {
            AccountCostumeUnlock = new HashSet<AccountCostumeUnlock>();
            AccountGenericUnlock = new HashSet<AccountGenericUnlock>();
            AccountKeybinding    = new HashSet<AccountKeybinding>();
            AccountPermission = new HashSet<AccountPermission>();
            AccountRole          = new HashSet<AccountRole>();
        }

        public uint Id { get; set; }
        public string Email { get; set; }
        public string S { get; set; }
        public string V { get; set; }
        public string GameToken { get; set; }
        public string SessionKey { get; set; }
        public DateTime CreateTime { get; set; }

        public virtual ICollection<AccountCostumeUnlock> AccountCostumeUnlock { get; set; }
        public virtual ICollection<AccountGenericUnlock> AccountGenericUnlock { get; set; }
        public virtual ICollection<AccountKeybinding> AccountKeybinding { get; set; }
        public virtual ICollection<AccountPermission> AccountPermission { get; set; }
        public virtual ICollection<AccountRole> AccountRole { get; set; }
    }
}

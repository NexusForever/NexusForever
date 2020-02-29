using System;
using System.Collections.Generic;

namespace NexusForever.Database.Auth.Model
{
    public class AccountModel
    {
        public uint Id { get; set; }
        public string Email { get; set; }
        public string S { get; set; }
        public string V { get; set; }
        public string GameToken { get; set; }
        public string SessionKey { get; set; }
        public DateTime CreateTime { get; set; }

        public ICollection<AccountCostumeUnlockModel> AccountCostumeUnlock { get; set; } = new HashSet<AccountCostumeUnlockModel>();
        public ICollection<AccountCurrencyModel> AccountCurrency { get; set; } = new HashSet<AccountCurrencyModel>();
        public ICollection<AccountEntitlementModel> AccountEntitlement { get; set; } = new HashSet<AccountEntitlementModel>();
        public ICollection<AccountGenericUnlockModel> AccountGenericUnlock { get; set; } = new HashSet<AccountGenericUnlockModel>();
        public ICollection<AccountKeybindingModel> AccountKeybinding { get; set; } = new HashSet<AccountKeybindingModel>();
    }
}

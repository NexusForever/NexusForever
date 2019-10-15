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

        public HashSet<AccountCostumeUnlockModel> CostumeUnlocks { get; set; } = new HashSet<AccountCostumeUnlockModel>();
        public HashSet<AccountCurrencyModel> Currencies { get; set; } = new HashSet<AccountCurrencyModel>();
        public HashSet<AccountEntitlementModel> Entitlements { get; set; } = new HashSet<AccountEntitlementModel>();
        public HashSet<AccountGenericUnlockModel> GenericUnlocks { get; set; } = new HashSet<AccountGenericUnlockModel>();
        public HashSet<AccountKeybindingModel> Keybindings { get; set; } = new HashSet<AccountKeybindingModel>();
    }
}

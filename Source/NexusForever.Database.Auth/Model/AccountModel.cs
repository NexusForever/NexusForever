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

        public ICollection<AccountCostumeUnlockModel> AccountCostumeUnlock { get; set; } = [];
        public ICollection<AccountCurrencyModel> AccountCurrency { get; set; } = [];
        public ICollection<AccountEntitlementModel> AccountEntitlement { get; set; } = [];
        public ICollection<AccountExternalReferenceModel> AccountExternalReference { get; set; } = [];
        public ICollection<AccountGenericUnlockModel> AccountGenericUnlock { get; set; } = [];
        public ICollection<AccountKeybindingModel> AccountKeybinding { get; set; } = [];
        public ICollection<AccountPermissionModel> AccountPermission { get; set; } = [];
        public ICollection<AccountRoleModel> AccountRole { get; set; } = [];
        public ICollection<AccountSuspensionModel> AccountSuspension { get; set; } = [];
    }
}

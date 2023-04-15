using NexusForever.Database;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Game.Abstract.Account;
using NexusForever.Game.Abstract.Account.Entitlement;
using NexusForever.Game.Entitlement;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Account.Entitlement
{
    public class AccountEntitlementManager : EntitlementManager<IAccountEntitlement>, IAccountEntitlementManager
    {
        private readonly IAccount account;

        /// <summary>
        /// Create a new <see cref="IAccountEntitlementManager"/> from existing database model.
        /// </summary>
        public AccountEntitlementManager(IAccount account, AccountModel model)
        {
            this.account = account;

            foreach (AccountEntitlementModel entitlementModel in model.AccountEntitlement)
            {
                EntitlementEntry entry = GameTableManager.Instance.Entitlement.GetEntry(entitlementModel.EntitlementId);
                if (entry == null)
                    throw new DatabaseDataException($"Account {model.Id} has invalid entitlement {entitlementModel.EntitlementId} stored!");

                var entitlement = new AccountEntitlement(account, entitlementModel, entry);
                entitlements.Add(entitlement.Type, entitlement);
            }
        }

        public void Save(AuthContext context)
        {
            foreach (IAccountEntitlement entitlement in entitlements.Values)
                entitlement.Save(context);
        }

        protected override bool CanUpdateEntitlement(EntitlementEntry entry, int value)
        {
            // only handle account entitlements in AccountEntitlementManager
            EntitlementFlags entitlementFlags = (EntitlementFlags)entry.Flags;
            if (entitlementFlags.HasFlag(EntitlementFlags.Character))
                return false;

            return base.CanUpdateEntitlement(entry, value);
        }

        protected override void UpdateEntitlement(EntitlementEntry entry, int value)
        {
            base.UpdateEntitlement(entry, value);

            // some reward property premium modifier entries use an existing entitlement values rather than static values
            // make sure we update these when the entitlement changes
            foreach (RewardPropertyPremiumModifierEntry modifierEntry in AssetManager.Instance.GetRewardPropertiesForTier(account.AccountTier)
                .Where(e => e.EntitlementIdModifierCount == entry.Id))
            {
                account.RewardPropertyManager.UpdateRewardProperty((RewardPropertyType)modifierEntry.RewardPropertyId, value, modifierEntry.RewardPropertyData);
            }
        }

        protected override IAccountEntitlement CreateEntitlement(EntitlementEntry type)
        {
            return new AccountEntitlement(account, type, 0);
        }

        protected override void SendEntitlement(IAccountEntitlement entitlement)
        {
            account.Session.EnqueueMessageEncrypted(entitlement.Build());
        }
    }
}

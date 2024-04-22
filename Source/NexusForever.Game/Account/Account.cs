using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Game.Abstract.Account;
using NexusForever.Game.Abstract.Account.Costume;
using NexusForever.Game.Abstract.Account.Currency;
using NexusForever.Game.Abstract.Account.Entitlement;
using NexusForever.Game.Abstract.Account.Reward;
using NexusForever.Game.Abstract.Account.Setting;
using NexusForever.Game.Abstract.Account.Unlock;
using NexusForever.Game.Abstract.RBAC;
using NexusForever.Game.Account.Costume;
using NexusForever.Game.Account.Currency;
using NexusForever.Game.Account.Entitlement;
using NexusForever.Game.Account.Reward;
using NexusForever.Game.Account.Setting;
using NexusForever.Game.Account.Unlock;
using NexusForever.Game.RBAC;
using NexusForever.Game.Static;
using NexusForever.Game.Static.RBAC;
using NexusForever.Network;

namespace NexusForever.Game.Account
{
    public class Account : IAccount
    {
        public uint Id { get; private set; }
        public string Email { get; private set; }

        public IAccountRBACManager RbacManager { get; private set; }
        public IGenericUnlockManager GenericUnlockManager { get; private set; }
        public IAccountCurrencyManager CurrencyManager { get; private set; }
        public IAccountEntitlementManager EntitlementManager { get; private set; }
        public IAccountCostumeManager CostumeManager { get; private set; }
        public IRewardPropertyManager RewardPropertyManager { get; private set; }
        public IAccountKeybindingManager KeybindingManager { get; private set; } 

        public AccountTier AccountTier => RbacManager.HasPermission(Permission.Signature) ? AccountTier.Signature : AccountTier.Basic;

        public IGameSession Session { get; private set; }

        /// <summary>
        /// Initialise <see cref="IAccount"/> with supplied  database model and <see cref="IGameSession"/>.
        /// </summary>
        public void Initialise(AccountModel model, IGameSession session)
        {
            if (Session != null)
                throw new InvalidOperationException();

            Id      = model.Id;
            Email   = model.Email;
            Session = session;

            RbacManager           = new AccountRBACManager(this, model);
            GenericUnlockManager  = new GenericUnlockManager(this, model);
            CurrencyManager       = new AccountCurrencyManager(this, model);
            EntitlementManager    = new AccountEntitlementManager(this, model);
            CostumeManager        = new AccountCostumeManager(this, model);
            RewardPropertyManager = new RewardPropertyManager(this);
            KeybindingManager     = new AccountKeybindingManager(model);
        }

        public void Save(AuthContext context)
        {
            RbacManager.Save(context);
            GenericUnlockManager.Save(context);
            CurrencyManager.Save(context);
            EntitlementManager.Save(context);
            CostumeManager.Save(context);
            KeybindingManager.Save(context);
        }
    }
}

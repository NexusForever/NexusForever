using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Game.Abstract.Account.Costume;
using NexusForever.Game.Abstract.Account.Currency;
using NexusForever.Game.Abstract.Account.Entitlement;
using NexusForever.Game.Abstract.Account.Reward;
using NexusForever.Game.Abstract.Account.Setting;
using NexusForever.Game.Abstract.Account.Unlock;
using NexusForever.Game.Abstract.RBAC;
using NexusForever.Game.Static;
using NexusForever.Network;

namespace NexusForever.Game.Abstract.Account
{
    public interface IAccount : IDatabaseAuth
    {
        uint Id { get; }
        string Email { get; }

        IAccountRBACManager RbacManager { get; }
        IGenericUnlockManager GenericUnlockManager { get; }
        IAccountCurrencyManager CurrencyManager { get; }
        IAccountEntitlementManager EntitlementManager { get; }
        IAccountCostumeManager CostumeManager { get; }
        IRewardPropertyManager RewardPropertyManager { get; }
        IAccountKeybindingManager KeybindingManager { get; }

        AccountTier AccountTier { get; }

        IGameSession Session { get; }

        /// <summary>
        /// Initialise <see cref="IAccount"/> with supplied  database model and <see cref="IGameSession"/>.
        /// </summary>
        void Initialise(AccountModel model, IGameSession session);
    }
}
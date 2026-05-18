using NexusForever.Database.Auth;
using NexusForever.Game.Abstract.Entitlement;

namespace NexusForever.Game.Abstract.Account.Entitlement
{
    public interface IAccountEntitlementManager : IEntitlementManager<IAccountEntitlement>, IDatabaseAuth
    {
    }
}

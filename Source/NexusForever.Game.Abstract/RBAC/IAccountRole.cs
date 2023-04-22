using NexusForever.Database;
using NexusForever.Database.Auth;

namespace NexusForever.Game.Abstract.RBAC
{
    public interface IAccountRole : IDatabaseAuth, IDatabaseState
    {
        uint Id { get; }
        IRBACRole Role { get; }
    }
}
using NexusForever.Database;
using NexusForever.Database.Auth;

namespace NexusForever.Game.Abstract.RBAC
{
    public interface IAccountPermission : IDatabaseAuth, IDatabaseState
    {
        uint Id { get; }
        IRBACPermission Permission { get; }
    }
}
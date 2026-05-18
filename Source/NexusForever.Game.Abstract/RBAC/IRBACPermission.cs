using NexusForever.Game.Static.RBAC;

namespace NexusForever.Game.Abstract.RBAC
{
    public interface IRBACPermission
    {
        Permission Permission { get; }
        string Name { get; }
    }
}
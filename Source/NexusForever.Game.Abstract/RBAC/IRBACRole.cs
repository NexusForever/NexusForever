using System.Collections.Immutable;
using NexusForever.Game.Static.RBAC;

namespace NexusForever.Game.Abstract.RBAC
{
    public interface IRBACRole
    {
        Role Role { get; set; }
        string Name { get; set; }
        RoleFlags Flags { get; set; }
        ImmutableDictionary<Permission, IRBACPermission> Permissions { get; }
    }
}
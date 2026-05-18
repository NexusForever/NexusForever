using System.Collections.Immutable;
using NexusForever.Database.Auth.Model;
using NexusForever.Game.Abstract.RBAC;
using NexusForever.Game.Static.RBAC;

namespace NexusForever.Game.RBAC
{
    public class RBACRole : IRBACRole
    {
        public Role Role { get; set; }
        public string Name { get; set; }
        public RoleFlags Flags { get; set; }
        public ImmutableDictionary<Permission, IRBACPermission> Permissions { get; }

        /// <summary>
        /// Create a new <see cref="IRBACRole"/> from an existing database model.
        /// </summary>
        public RBACRole(RoleModel model, IEnumerable<IRBACPermission> permissions)
        {
            Role        = (Role)model.Id;
            Name        = model.Name;
            Flags       = (RoleFlags)model.Flags;
            Permissions = permissions.ToImmutableDictionary(s => s.Permission);
        }
    }
}

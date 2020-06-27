using System.Collections.Generic;
using System.Collections.Immutable;
using NexusForever.Database.Auth.Model;
using NexusForever.WorldServer.Game.RBAC.Static;

namespace NexusForever.WorldServer.Game.RBAC
{
    public class RBACRole
    {
        public Role Role { get; set; }
        public string Name { get; set; }
        public RoleFlags Flags { get; set; }
        public ImmutableDictionary<Permission, RBACPermission> Permissions { get; }

        /// <summary>
        /// Create a new <see cref="RBACRole"/> from an existing database model.
        /// </summary>
        public RBACRole(RoleModel model, IEnumerable<KeyValuePair<Permission, RBACPermission>> permissions)
        {
            Role        = (Role)model.Id;
            Name        = model.Name;
            Flags       = (RoleFlags)model.Flags;
            Permissions = permissions.ToImmutableDictionary();
        }
    }
}

using System.Collections.Immutable;
using NexusForever.Database;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Game.Abstract.RBAC;
using NexusForever.Game.Static.RBAC;
using NexusForever.Shared;
using NLog;

namespace NexusForever.Game.RBAC
{
    public sealed class RBACManager : Singleton<RBACManager>, IRBACManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private ImmutableDictionary<Permission, IRBACPermission> permissions;
        private ImmutableDictionary<Role, IRBACRole> roles;

        private RBACManager()
        {
        }

        public void Initialise()
        {
            log.Info("Initialising RBAC permissions...");

            var permissionBuilder = ImmutableDictionary.CreateBuilder<Permission, IRBACPermission>();
            foreach (PermissionModel permissionModel in DatabaseManager.Instance.GetDatabase<AuthDatabase>().GetPermissions())
            {
                var permission = new RBACPermission(permissionModel);
                permissionBuilder.Add(permission.Permission, permission);
            }

            permissions = permissionBuilder.ToImmutable();

            var roleBuilder = ImmutableDictionary.CreateBuilder<Role, IRBACRole>();
            foreach (RoleModel roleModel in DatabaseManager.Instance.GetDatabase<AuthDatabase>().GetRoles())
            {
                // check if permissions for role exist
                if (roleModel.RolePermission.Any(p => GetPermission((Permission)p.PermissionId) == null))
                    throw new DatabaseDataException($"Role {roleModel.Flags}");

                IEnumerable<IRBACPermission> rolePermissions = roleModel.RolePermission
                    .Select(p => GetPermission((Permission)p.PermissionId));

                // all permissions are included
                RoleFlags flags = (RoleFlags)roleModel.Flags;
                if ((flags & RoleFlags.Inclusive) != 0)
                {
                    var role = new RBACRole(roleModel, rolePermissions);
                    roleBuilder.Add(role.Role, role);
                }
                // all permissions are excluded
                // this is used when a role will have all permission except a few
                else if ((flags & RoleFlags.Exclusive) != 0)
                {
                    var role = new RBACRole(roleModel, permissions.Values.Except(rolePermissions));
                    roleBuilder.Add(role.Role, role);
                }
            }

            roles = roleBuilder.ToImmutable();

            log.Info($"Loaded {permissions.Count} permission(s) in {roles.Count} role(s).");
        }

        /// <summary>
        /// Return <see cref="IRBACPermission"/> with supplied <see cref="Permission"/> id.
        /// </summary>
        public IRBACPermission GetPermission(Permission permission)
        {
            return permissions.TryGetValue(permission, out IRBACPermission rbacPermission) ? rbacPermission : null;
        }

        /// <summary>
        /// Return <see cref="IRBACRole"/> with supplied <see cref="Role"/> id.
        /// </summary>
        public IRBACRole GetRole(Role role)
        {
            return roles.TryGetValue(role, out IRBACRole rbacRole) ? rbacRole : null;
        }
    }
}

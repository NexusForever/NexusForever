using NexusForever.Database;
using NexusForever.Database.Auth.Model;
using NexusForever.Shared;
using NexusForever.Shared.Database;
using NexusForever.WorldServer.Game.RBAC.Static;
using System.Collections.Immutable;
using System.Linq;

namespace NexusForever.WorldServer.Game.RBAC
{
    public sealed class RBACManager : AbstractManager<RBACManager>
    {
        private ImmutableDictionary<Permission, RBACPermission> permissions;
        private ImmutableDictionary<Role, RBACRole> roles;

        private RBACManager()
        {
        }

        public override RBACManager Initialise()
        {
            Log.Info("Initialising RBAC permissions...");

            var permissionBuilder = ImmutableDictionary.CreateBuilder<Permission, RBACPermission>();
            foreach (PermissionModel permissionModel in DatabaseManager.Instance.AuthDatabase.GetPermissions())
            {
                var permission = new RBACPermission(permissionModel);
                permissionBuilder.Add(permission.Permission, permission);
            }

            permissions = permissionBuilder.ToImmutable();

            var roleBuilder = ImmutableDictionary.CreateBuilder<Role, RBACRole>();
            foreach (RoleModel roleModel in DatabaseManager.Instance.AuthDatabase.GetRoles())
            {
                // check if permissions for role exist
                if (roleModel.RolePermission.Any(p => GetPermission((Permission)p.PermissionId) == null))
                    throw new DatabaseDataException($"Role {roleModel.Flags}");

                // all permissions are included
                RoleFlags flags = (RoleFlags)roleModel.Flags;
                if ((flags & RoleFlags.Inclusive) != 0)
                {
                    var role = new RBACRole(roleModel,
                        roleModel.RolePermission
                            .Select(p => GetPermission((Permission)p.PermissionId))
                            .ToImmutableDictionary(p => p.Permission, p => p));
                    roleBuilder.Add(role.Role, role);
                }
                // all permissions are excluded
                // this is used when a role will have all permission except a few
                else if ((flags & RoleFlags.Exclusive) != 0)
                {
                    ImmutableDictionary<Permission, RBACPermission> except = roleModel.RolePermission
                        .Select(p => GetPermission((Permission)p.PermissionId))
                        .ToImmutableDictionary(p => p.Permission, p => p);

                    var role = new RBACRole(roleModel,
                        permissions
                            .Except(except)
                            .ToImmutableDictionary(p => p.Key, p => p.Value));
                    roleBuilder.Add(role.Role, role);
                }
            }

            roles = roleBuilder.ToImmutable();

            Log.Info($"Loaded {permissions.Count} permission(s) in {roles.Count} role(s).");
            return Instance;
        }

        /// <summary>
        /// Return <see cref="RBACPermission"/> with supplied <see cref="Permission"/> id.
        /// </summary>
        public RBACPermission GetPermission(Permission permission)
        {
            return permissions.TryGetValue(permission, out RBACPermission rbacPermission) ? rbacPermission : null;
        }

        /// <summary>
        /// Return <see cref="RBACRole"/> with supplied <see cref="Role"/> id.
        /// </summary>
        public RBACRole GetRole(Role role)
        {
            return roles.TryGetValue(role, out RBACRole rbacRole) ? rbacRole : null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NexusForever.Database;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.WorldServer.Game.RBAC.Static;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Game.RBAC
{
    public class AccountRBACManager : ISaveAuth
    {
        private readonly WorldSession session;

        private readonly Dictionary<Permission, AccountPermission> permissions = new();
        private readonly Dictionary<Role, AccountRole> roles = new();

        /// <summary>
        /// Create a new <see cref="AccountRBACManager"/> from an existing database model.
        /// </summary>
        public AccountRBACManager(WorldSession session, AccountModel model)
        {
            this.session = session;

            foreach (AccountPermissionModel permissionModel in model.AccountPermission)
            {
                RBACPermission rbacPermission = RBACManager.Instance.GetPermission((Permission)permissionModel.PermissionId);
                if (rbacPermission == null)
                    throw new DatabaseDataException($"Account {model.Id} has invalid permission {permissionModel.PermissionId}!");

                permissions.Add(rbacPermission.Permission, new AccountPermission(permissionModel, rbacPermission));
            }

            foreach (AccountRoleModel roleModel in model.AccountRole)
            {
                RBACRole rbacRole = RBACManager.Instance.GetRole((Role)roleModel.RoleId);
                if (rbacRole == null)
                    throw new DatabaseDataException($"Account {model.Id} has invalid role {roleModel.RoleId}!");

                roles.Add(rbacRole.Role, new AccountRole(roleModel, rbacRole));
            }
        }

        /// <summary>
        /// Returns if account has <see cref="RBACPermission"/> with supplied <see cref="Permission"/> id.
        /// </summary>
        /// <remarks>
        /// This will return regardless of whether the permission is granted individually or from a role.
        /// </remarks>
        public bool HasPermission(Permission permission)
        {
            return roles.Values
                .Any(role => !role.PendingDelete && role.Role.Permissions.ContainsKey(permission))
                   || permissions.TryGetValue(permission, out AccountPermission accountPermission) && !accountPermission.PendingDelete;
        }

        /// <summary>
        /// Returns if account has <see cref="RBACRole"/> with supplied <see cref="Role"/> id.
        /// </summary>
        public bool HasRole(Role role)
        {
            return roles.TryGetValue(role, out AccountRole accountRole) && !accountRole.PendingDelete;
        }

        /// <summary>
        /// Grant <see cref="RBACPermission"/> with supplied <see cref="Permission"/> id to account.
        /// </summary>
        public void GrantPermission(Permission permission)
        {
            RBACPermission rbacPermission = RBACManager.Instance.GetPermission(permission);
            if (rbacPermission == null)
                throw new ArgumentException($"Failed to grant permission to account {session.Account.Id}, {permission} isn't a valid permission!");

            if (!permissions.TryGetValue(permission, out AccountPermission accountPermission))
                permissions.Add(rbacPermission.Permission, new AccountPermission(session.Account.Id, rbacPermission));
            else
            {
                // we have this permission but it's pending a delete from the database, reuse the model
                if (accountPermission.PendingDelete)
                    accountPermission.EnqueueDelete(false);
                else
                    throw new ArgumentException($"Failed to grant permission to account {session.Account.Id}, account already has permission!");
            }
        }

        /// <summary>
        /// Revoke <see cref="RBACPermission"/> with supplied <see cref="Permission"/> id from account.
        /// </summary>
        public void RevokePermission(Permission permission)
        {
            if (!permissions.TryGetValue(permission, out AccountPermission accountPermission) || accountPermission.PendingDelete)
                throw new ArgumentException($"Failed to revoke permission from account {session.Account.Id}, account doesn't have permission {permission}!");

            // permission has yet to be saved to database, remove it right away
            if (accountPermission.PendingCreate)
                permissions.Remove(permission);
            // permission has been saved to database, delay remove it
            else
                accountPermission.EnqueueDelete(true);
        }

        /// <summary>
        /// Grant <see cref="RBACRole"/> with supplied <see cref="Role"/> id to account.
        /// </summary>
        public void GrantRole(Role role)
        {
            RBACRole rbacRole = RBACManager.Instance.GetRole(role);
            if (rbacRole == null)
                throw new ArgumentException($"Failed to grant role to account {session.Account.Id}, {role} isn't a valid role!");

            if (!roles.TryGetValue(role, out AccountRole accountRole))
                roles.Add(rbacRole.Role, new AccountRole(session.Account.Id, rbacRole));
            else
            {
                // we have this role but it's pending a delete from the database, reuse the model
                if (accountRole.PendingDelete)
                    accountRole.EnqueueDelete(false);
                else
                    throw new ArgumentException($"Failed to grant role to account {session.Account.Id}, account already has role!");
            }
        }

        /// <summary>
        /// Revoke <see cref="RBACRole"/> with supplied <see cref="Role"/> id from account.
        /// </summary>
        public void RevokeRole(Role role)
        {
            if (!roles.TryGetValue(role, out AccountRole accountRole) || accountRole.PendingDelete)
                throw new ArgumentException($"Failed to revoke role from account {session.Account.Id}, account doesn't have role {role}!");

            // role has yet to be saved to database, remove it right away
            if (accountRole.PendingCreate)
                roles.Remove(role);
            // role has been saved to database, delay remove it
            else
                accountRole.EnqueueDelete(true);
        }

        /// <summary>
        /// Return all <see cref="Permission"/>'s from <see cref="RBACRole"/>'s and single <see cref="RBACPermission"/>'s.
        /// </summary>
        public ImmutableHashSet<Permission> GetPermissions()
        {
            var permissionSet = ImmutableHashSet.CreateBuilder<Permission>();

            foreach (Permission permission in permissions.Keys)
                permissionSet.Add(permission);
            foreach (Permission permission in roles
                .SelectMany(rolePair => rolePair.Value.Role.Permissions.Keys))
                permissionSet.Add(permission);

            return permissionSet.ToImmutable();
        }

        public void Save(AuthContext context)
        {
            foreach (AccountPermission permission in permissions.Values)
                permission.Save(context);
            foreach (AccountRole role in roles.Values)
                role.Save(context);
        }
    }
}

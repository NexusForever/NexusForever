﻿using System.Collections.Immutable;
using NexusForever.Database;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Game.Abstract.Account;
using NexusForever.Game.Abstract.RBAC;
using NexusForever.Game.Configuration.Model;
using NexusForever.Game.Static.RBAC;
using NexusForever.Shared.Configuration;

namespace NexusForever.Game.RBAC
{
    public class AccountRBACManager : IAccountRBACManager
    {
        private readonly IAccount account;

        private readonly Dictionary<Permission, IAccountPermission> permissions = new();
        private readonly Dictionary<Role, IAccountRole> roles = new();

        private Role defaultRole = (SharedConfiguration.Instance.Get<RealmConfig>().DefaultRole ?? Role.Player);

        /// <summary>
        /// Create a new <see cref="IAccountRBACManager"/> from an existing database model.
        /// </summary>
        public AccountRBACManager(IAccount account, AccountModel model)
        {
            this.account = account;

            foreach (AccountPermissionModel permissionModel in model.AccountPermission)
            {
                IRBACPermission rbacPermission = RBACManager.Instance.GetPermission((Permission)permissionModel.PermissionId);
                if (rbacPermission == null)
                    throw new DatabaseDataException($"Account {model.Id} has invalid permission {permissionModel.PermissionId}!");

                permissions.Add(rbacPermission.Permission, new AccountPermission(permissionModel, rbacPermission));
            }

            foreach (AccountRoleModel roleModel in model.AccountRole)
            {
                IRBACRole rbacRole = RBACManager.Instance.GetRole((Role)roleModel.RoleId);
                if (rbacRole == null)
                    throw new DatabaseDataException($"Account {model.Id} has invalid role {roleModel.RoleId}!");

                roles.Add(rbacRole.Role, new AccountRole(roleModel, rbacRole));
            }

            if (roles.Count == 0)
            {
                IRBACRole rbacRole = RBACManager.Instance.GetRole(defaultRole);
                if (rbacRole == null)
                    throw new DatabaseDataException($"Account {model.Id} has invalid role {defaultRole}!");

                roles.Add(rbacRole.Role, new AccountRole(model.Id, rbacRole));
            }
        }

        /// <summary>
        /// Returns if account has <see cref="IRBACPermission"/> with supplied <see cref="Permission"/> id.
        /// </summary>
        /// <remarks>
        /// This will return regardless of whether the permission is granted individually or from a role.
        /// </remarks>
        public bool HasPermission(Permission permission)
        {
            return roles.Values
                .Any(role => !role.PendingDelete && role.Role.Permissions.ContainsKey(permission))
                   || permissions.TryGetValue(permission, out IAccountPermission accountPermission) && !accountPermission.PendingDelete;
        }

        /// <summary>
        /// Returns if account has <see cref="IRBACRole"/> with supplied <see cref="Role"/> id.
        /// </summary>
        public bool HasRole(Role role)
        {
            return roles.TryGetValue(role, out IAccountRole accountRole) && !accountRole.PendingDelete;
        }

        /// <summary>
        /// Grant <see cref="IRBACPermission"/> with supplied <see cref="Permission"/> id to account.
        /// </summary>
        public void GrantPermission(Permission permission)
        {
            IRBACPermission rbacPermission = RBACManager.Instance.GetPermission(permission);
            if (rbacPermission == null)
                throw new ArgumentException($"Failed to grant permission to account {account.Id}, {permission} isn't a valid permission!");

            if (!permissions.TryGetValue(permission, out IAccountPermission accountPermission))
                permissions.Add(rbacPermission.Permission, new AccountPermission(account.Id, rbacPermission));
            else
            {
                // we have this permission but it's pending a delete from the database, reuse the model
                if (accountPermission.PendingDelete)
                    accountPermission.EnqueueDelete(false);
                else
                    throw new ArgumentException($"Failed to grant permission to account {account.Id}, account already has permission!");
            }
        }

        /// <summary>
        /// Revoke <see cref="IRBACPermission"/> with supplied <see cref="Permission"/> id from account.
        /// </summary>
        public void RevokePermission(Permission permission)
        {
            if (!permissions.TryGetValue(permission, out IAccountPermission accountPermission) || accountPermission.PendingDelete)
                throw new ArgumentException($"Failed to revoke permission from account {account.Id}, account doesn't have permission {permission}!");

            // permission has yet to be saved to database, remove it right away
            if (accountPermission.PendingCreate)
                permissions.Remove(permission);
            // permission has been saved to database, delay remove it
            else
                accountPermission.EnqueueDelete(true);
        }

        /// <summary>
        /// Grant <see cref="IRBACRole"/> with supplied <see cref="Role"/> id to account.
        /// </summary>
        public void GrantRole(Role role)
        {
            IRBACRole rbacRole = RBACManager.Instance.GetRole(role);
            if (rbacRole == null)
                throw new ArgumentException($"Failed to grant role to account {account.Id}, {role} isn't a valid role!");

            if (!roles.TryGetValue(role, out IAccountRole accountRole))
                roles.Add(rbacRole.Role, new AccountRole(account.Id, rbacRole));
            else
            {
                // we have this role but it's pending a delete from the database, reuse the model
                if (accountRole.PendingDelete)
                    accountRole.EnqueueDelete(false);
                else
                    throw new ArgumentException($"Failed to grant role to account {account.Id}, account already has role!");
            }
        }

        /// <summary>
        /// Revoke <see cref="IRBACRole"/> with supplied <see cref="Role"/> id from account.
        /// </summary>
        public void RevokeRole(Role role)
        {
            if (!roles.TryGetValue(role, out IAccountRole accountRole) || accountRole.PendingDelete)
                throw new ArgumentException($"Failed to revoke role from account {account.Id}, account doesn't have role {role}!");

            // role has yet to be saved to database, remove it right away
            if (accountRole.PendingCreate)
                roles.Remove(role);
            // role has been saved to database, delay remove it
            else
                accountRole.EnqueueDelete(true);
        }

        /// <summary>
        /// Return all <see cref="Permission"/>'s from <see cref="IRBACRole"/>'s and single <see cref="IRBACPermission"/>'s.
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
            foreach (IAccountPermission permission in permissions.Values)
                permission.Save(context);
            foreach (IAccountRole role in roles.Values)
                role.Save(context);
        }
    }
}

using System.Collections.Immutable;
using NexusForever.Database.Auth;
using NexusForever.Game.Static.RBAC;

namespace NexusForever.Game.Abstract.RBAC
{
    public interface IAccountRBACManager : IDatabaseAuth
    {
        /// <summary>
        /// Returns if account has <see cref="IRBACPermission"/> with supplied <see cref="Permission"/> id.
        /// </summary>
        /// <remarks>
        /// This will return regardless of whether the permission is granted individually or from a role.
        /// </remarks>
        bool HasPermission(Permission permission);

        /// <summary>
        /// Returns if account has <see cref="IRBACRole"/> with supplied <see cref="Role"/> id.
        /// </summary>
        bool HasRole(Role role);

        /// <summary>
        /// Grant <see cref="IRBACPermission"/> with supplied <see cref="Permission"/> id to account.
        /// </summary>
        void GrantPermission(Permission permission);

        /// <summary>
        /// Revoke <see cref="IRBACPermission"/> with supplied <see cref="Permission"/> id from account.
        /// </summary>
        void RevokePermission(Permission permission);

        /// <summary>
        /// Grant <see cref="IRBACRole"/> with supplied <see cref="Role"/> id to account.
        /// </summary>
        void GrantRole(Role role);

        /// <summary>
        /// Revoke <see cref="IRBACRole"/> with supplied <see cref="Role"/> id from account.
        /// </summary>
        void RevokeRole(Role role);

        /// <summary>
        /// Return all <see cref="Permission"/>'s from <see cref="IRBACRole"/>'s and single <see cref="IRBACPermission"/>'s.
        /// </summary>
        ImmutableHashSet<Permission> GetPermissions();
    }
}
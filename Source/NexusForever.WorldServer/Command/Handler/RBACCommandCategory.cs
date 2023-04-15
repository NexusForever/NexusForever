using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.RBAC;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.RBAC, "A collection of commands of manage RBAC permissions and roles.", "rbac")]
    [CommandTarget(typeof(IPlayer))]
    public class RBACCommandCategory : CommandCategory
    {
        [Command(Permission.RBACAccount, "A collection of commands to manage RBAC permissions and roles for an account.", "account")]
        public class RBACAccountCommandCategory : CommandCategory
        {
            [Command(Permission.RBACAccountPermission, "A collection of commands to manage RBAC permissions for an account.", "permission")]
            public class RBACAccountPermissionCommandCategory : CommandCategory
            {
                [Command(Permission.RBACAccountPermissionGrant, "Grant permission to account", "grant")]
                public void HandleRBACAccountPermissionGrant(ICommandContext context,
                    [Parameter("Permission to grant", ParameterFlags.None, typeof(EnumParameterConverter<Permission>))]
                    Permission permission)
                {
                    context.GetTargetOrInvoker<IPlayer>().Account.RbacManager.GrantPermission(permission);
                }

                [Command(Permission.RBACAccountPermissionRevoke, "", "revoke")]
                public void HandleRBACAccountPermissionRevoke(ICommandContext context,
                    [Parameter("Permission to revoke", ParameterFlags.None, typeof(EnumParameterConverter<Permission>))]
                    Permission permission)
                {
                    context.GetTargetOrInvoker<IPlayer>().Account.RbacManager.RevokePermission(permission);
                }
            }

            [Command(Permission.RBACAccountRole, "A collection of commands to manage RBAC roles for an account.", "role")]
            public class RBACAccountRoleCommandCategory : CommandCategory
            {
                [Command(Permission.RBACAccountRoleGrant, "Grant role to account", "grant")]
                public void HandleRBACAccountRoleGrant(ICommandContext context,
                    [Parameter("Role to grant", ParameterFlags.None, typeof(EnumParameterConverter<Role>))]
                    Role role)
                {
                    context.GetTargetOrInvoker<IPlayer>().Account.RbacManager.GrantRole(role);
                }

                [Command(Permission.RBACAccountRoleRevoke, "Remove role from account", "revoke")]
                public void HandleRBACAccountRoleRevoke(ICommandContext context,
                    [Parameter("Role to revoke", ParameterFlags.None, typeof(EnumParameterConverter<Role>))]
                    Role role)
                {
                    context.GetTargetOrInvoker<IPlayer>().Account.RbacManager.RevokeRole(role);
                }
            }
        }
    }
}

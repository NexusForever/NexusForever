using System;
using System.Collections.Generic;
using System.Text;
using AuthDatabase = NexusForever.Shared.Database.Auth.AuthDatabase;
using AccountPermissionModel = NexusForever.Shared.Database.Auth.Model.AccountPermission;
using AccountRoleModel = NexusForever.Shared.Database.Auth.Model.AccountRole;
using RoleModel = NexusForever.Shared.Database.Auth.Model.Role;
using System.Collections.Immutable;
using NexusForever.WorldServer.Game.Account.Static;
using NexusForever.WorldServer.Network;
using NLog;
using System.Linq;
using NexusForever.Shared.Configuration;

namespace NexusForever.WorldServer.Game.Account
{
    public static class RoleManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static readonly Dictionary<ulong, Role> Roles = new Dictionary<ulong, Role>();

        private static ulong DefaultRole = 0;

        public static void Initialise()
        {
            LoadRoles();
            DefaultRole = ConfigurationManager<WorldServerConfiguration>.Config.DefaultRole;

            log.Info($"Initialised {Roles.Count} roles");
        }

        private static void LoadRoles()
        {
            ImmutableList<RoleModel> roleModels = AuthDatabase.GetRoles();

            foreach(RoleModel roleModel in roleModels)
            {
                Role role = new Role(roleModel);
                Roles.Add(role.Id, role);
            }
        }

        public static bool HasPermission(WorldSession session, Permission permission)
        {
            bool hasPermission = false;

            if (permission == Permission.None)
                hasPermission = true;

            if (session.Account.AccountPermission.Select(x => (Permission)x.PermissionId).ToList().Contains(permission))
                hasPermission = true;

            foreach (AccountRoleModel accountRole in session.Account.AccountRole)
            {
                if (hasPermission == true)
                    break;

                if (Roles.TryGetValue(accountRole.RoleId, out Role role))
                    hasPermission = role.HasPermission(permission);
            }

            if (!hasPermission && DefaultRole > 0 && Roles.TryGetValue(DefaultRole, out Role defaultRole))
                hasPermission = defaultRole.HasPermission(permission);

            return hasPermission;
        }
    }
}

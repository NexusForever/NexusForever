using NexusForever.WorldServer.Game.Account.Static;
using System;
using System.Collections.Generic;
using System.Text;
using RoleModel = NexusForever.Shared.Database.Auth.Model.Role;
using RolePermissionModel = NexusForever.Shared.Database.Auth.Model.RolePermission;
using NLog;

namespace NexusForever.WorldServer.Game.Account
{
    public class Role
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public ulong Id { get; }
        public string Name { get; }

        private List<Permission> permissions = new List<Permission>();

        public Role(RoleModel model)
        {
            Id = model.Id;
            Name = model.Name;

            foreach(RolePermissionModel permissionModel in model.RolePermission)
            {
                if (!Enum.IsDefined(typeof(Permission), permissionModel.PermissionId))
                    continue;

                log.Trace($"Adding permission {(Permission)permissionModel.PermissionId} to {Name}");
                permissions.Add((Permission)permissionModel.PermissionId);
            }
        }

        public bool HasPermission(Permission permission)
        {
            if (permissions.Contains(Permission.Everything))
                return true;

            return permissions.Contains(permission);
        }
    }
}

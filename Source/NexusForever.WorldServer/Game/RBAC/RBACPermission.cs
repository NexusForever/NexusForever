using NexusForever.Database.Auth.Model;
using NexusForever.WorldServer.Game.RBAC.Static;

namespace NexusForever.WorldServer.Game.RBAC
{
    public class RBACPermission
    {
        public Permission Permission { get; }
        public string Name { get; }

        /// <summary>
        /// Create a new <see cref="RBACPermission"/> from an existing database model.
        /// </summary>
        public RBACPermission(PermissionModel model)
        {
            Permission = (Permission)model.Id;
            Name       = model.Name;
        }
    }
}

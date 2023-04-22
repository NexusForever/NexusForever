using NexusForever.Database.Auth.Model;
using NexusForever.Game.Abstract.RBAC;
using NexusForever.Game.Static.RBAC;

namespace NexusForever.Game.RBAC
{
    public class RBACPermission : IRBACPermission
    {
        public Permission Permission { get; }
        public string Name { get; }

        /// <summary>
        /// Create a new <see cref="IRBACPermission"/> from an existing database model.
        /// </summary>
        public RBACPermission(PermissionModel model)
        {
            Permission = (Permission)model.Id;
            Name       = model.Name;
        }
    }
}

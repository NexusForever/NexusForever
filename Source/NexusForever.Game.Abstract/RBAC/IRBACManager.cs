using NexusForever.Game.Static.RBAC;

namespace NexusForever.Game.Abstract.RBAC
{
    public interface IRBACManager
    {
        void Initialise();

        /// <summary>
        /// Return <see cref="IRBACPermission"/> with supplied <see cref="Permission"/> id.
        /// </summary>
        IRBACPermission GetPermission(Permission permission);

        /// <summary>
        /// Return <see cref="IRBACRole"/> with supplied <see cref="Role"/> id.
        /// </summary>
        IRBACRole GetRole(Role role);
    }
}
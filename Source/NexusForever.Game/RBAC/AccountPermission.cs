using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Game.Abstract.RBAC;

namespace NexusForever.Game.RBAC
{
    public class AccountPermission : IAccountPermission
    {
        [Flags]
        private enum SaveMask
        {
            None   = 0x00,
            Create = 0x01,
            Delete = 0x02
        }

        public uint Id { get; }
        public IRBACPermission Permission { get; }

        /// <summary>
        /// Returns if <see cref="IAccountPermission"/> is enqueued to be saved to the database.
        /// </summary>
        public bool PendingCreate => (saveMask & SaveMask.Create) != 0;

        /// <summary>
        /// Returns if <see cref="IAccountPermission"/> is enqueued to be deleted from the database.
        /// </summary>
        public bool PendingDelete => (saveMask & SaveMask.Delete) != 0;

        private SaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="IAccountPermission"/> from an existing database model.
        /// </summary>
        public AccountPermission(AccountPermissionModel model, IRBACPermission permission)
        {
            Id         = model.Id;
            Permission = permission;
            saveMask   = SaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="IAccountPermission"/> from a <see cref="IRBACPermission"/>.
        /// </summary>
        public AccountPermission(uint id, IRBACPermission permission)
        {
            Id         = id;
            Permission = permission;
            saveMask   = SaveMask.Create;
        }

        public void Save(AuthContext context)
        {
            if (saveMask == SaveMask.None)
                return;

            var model = new AccountPermissionModel
            {
                Id           = Id,
                PermissionId = (uint)Permission.Permission
            };

            if ((saveMask & SaveMask.Create) != 0)
                context.Add(model);
            else
                context.Remove(model);

            saveMask = SaveMask.None;
        }

        /// <summary>
        /// Enqueue <see cref="IAccountPermission"/> to be deleted from the database.
        /// </summary>
        public void EnqueueDelete(bool delete)
        {
            if (delete)
                saveMask |= SaveMask.Delete;
            else
                saveMask &= ~SaveMask.Delete;
        }
    }
}

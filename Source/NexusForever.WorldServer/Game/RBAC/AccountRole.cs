using System;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;

namespace NexusForever.WorldServer.Game.RBAC
{
    public class AccountRole : ISaveAuth
    {
        [Flags]
        private enum SaveMask
        {
            None   = 0x00,
            Create = 0x01,
            Delete = 0x02
        }

        public uint Id { get; }
        public RBACRole Role { get; }

        /// <summary>
        /// Returns if <see cref="AccountRole"/> is enqueued to be saved to the database.
        /// </summary>
        public bool PendingCreate => (saveMask & SaveMask.Create) != 0;

        /// <summary>
        /// Returns if <see cref="AccountRole"/> is enqueued to be deleted from the database.
        /// </summary>
        public bool PendingDelete => (saveMask & SaveMask.Delete) != 0;

        private SaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="AccountRole"/> from an existing database model.
        /// </summary>
        public AccountRole(AccountRoleModel model, RBACRole role)
        {
            Id       = model.Id;
            Role     = role;
            saveMask = SaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="AccountRole"/> from a <see cref="RBACRole"/>.
        /// </summary>
        public AccountRole(uint id, RBACRole role)
        {
            Id       = id;
            Role     = role;
            saveMask = SaveMask.Create;
        }

        public void Save(AuthContext context)
        {
            if (saveMask == SaveMask.None)
                return;

            var model = new AccountRoleModel
            {
                Id     = Id,
                RoleId = (uint)Role.Role
            };

            if ((saveMask & SaveMask.Create) != 0)
                context.Add(model);
            else
                context.Remove(model);

            saveMask = SaveMask.None;
        }

        /// <summary>
        /// Enqueue <see cref="AccountRole"/> to be deleted from the database.
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

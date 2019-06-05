using Microsoft.EntityFrameworkCore;
using NexusForever.Shared.Database;
using NexusForever.Shared.Database.Auth.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using AccountModel = NexusForever.Shared.Database.Auth.Model.Account;

namespace NexusForever.WorldServer.Game.Entity
{
    public class CostumeUnlock : ISaveAuth
    {
        public uint ItemId { get; }

        private readonly uint accountId;
        private CostumeUnlockSaveMask saveMask;

        public bool PendingDelete => (saveMask & CostumeUnlockSaveMask.Delete) != 0;

        /// <summary>
        /// Create a new <see cref="CostumeUnlock"/> from an existing <see cref="AccountCostumeUnlock"/> database model.
        /// </summary>
        public CostumeUnlock(AccountCostumeUnlock model)
        {
            ItemId    = model.ItemId;
            accountId = model.Id;
        }

        /// <summary>
        /// Create a new <see cref="CostumeUnlock"/> from supplied item id.
        /// </summary>
        public CostumeUnlock(AccountModel account, uint itemId)
        {
            ItemId    = itemId;
            accountId = account.Id;
            saveMask  = CostumeUnlockSaveMask.Create;
        }

        public void Save(AuthContext context)
        {
            if (saveMask == CostumeUnlockSaveMask.None)
                return;

            var model = new AccountCostumeUnlock
            {
                Id     = accountId,
                ItemId = ItemId
            };

            if ((saveMask & CostumeUnlockSaveMask.Create) != 0)
                context.Add(model);
            else
                context.Entry(model).State = EntityState.Deleted;

            saveMask = CostumeUnlockSaveMask.None;
        }

        /// <summary>
        /// Enqueue <see cref="CostumeUnlock"/> to be deleted from the database.
        /// </summary>
        public void EnqueueDelete(bool set)
        {
            if (set)
                saveMask |= CostumeUnlockSaveMask.Delete;
            else
                saveMask &= ~CostumeUnlockSaveMask.Delete;
        }
    }
}

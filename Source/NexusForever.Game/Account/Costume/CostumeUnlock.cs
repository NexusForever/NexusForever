using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Game.Abstract.Account;
using NexusForever.Game.Abstract.Account.Costume;

namespace NexusForever.Game.Account.Costume
{
    public class CostumeUnlock : ICostumeUnlock
    {
        [Flags]
        public enum CostumeUnlockSaveMask
        {
            None   = 0x00,
            Create = 0x01,
            Delete = 0x02
        }

        public uint ItemId { get; }

        private readonly uint accountId;
        private CostumeUnlockSaveMask saveMask;

        public bool PendingCreate => (saveMask & CostumeUnlockSaveMask.Create) != 0;
        public bool PendingDelete => (saveMask & CostumeUnlockSaveMask.Delete) != 0;

        /// <summary>
        /// Create a new <see cref="ICostumeUnlock"/> from an existing <see cref="AccountCostumeUnlockModel"/> database model.
        /// </summary>
        public CostumeUnlock(AccountCostumeUnlockModel model)
        {
            ItemId    = model.ItemId;
            accountId = model.Id;
        }

        /// <summary>
        /// Create a new <see cref="ICostumeUnlock"/> from supplied item id.
        /// </summary>
        public CostumeUnlock(IAccount account, uint itemId)
        {
            ItemId    = itemId;
            accountId = account.Id;
            saveMask  = CostumeUnlockSaveMask.Create;
        }

        public void Save(AuthContext context)
        {
            if (saveMask == CostumeUnlockSaveMask.None)
                return;

            var model = new AccountCostumeUnlockModel
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
        /// Enqueue <see cref="ICostumeUnlock"/> to be deleted from the database.
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

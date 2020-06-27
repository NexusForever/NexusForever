using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Entity
{
    public class GenericUnlock : ISaveAuth
    {
        public GenericUnlockEntryEntry Entry { get; }
        public GenericUnlockType Type => (GenericUnlockType)Entry.GenericUnlockTypeEnum;

        private readonly uint accountId;
        private bool isDirty;

        /// <summary>
        /// Create a new <see cref="CostumeUnlock"/> from existing <see cref="AccountGenericUnlockModel"/> database model.
        /// </summary>
        public GenericUnlock(AccountGenericUnlockModel model)
        {
            Entry     = GameTableManager.Instance.GenericUnlockEntry.GetEntry(model.Entry);
            accountId = model.Id;
        }

        /// <summary>
        /// Create a new <see cref="CostumeUnlock"/> from supplied <see cref="GenericUnlockEntryEntry"/>.
        /// </summary>
        public GenericUnlock(AccountModel account, GenericUnlockEntryEntry entry)
        {
            Entry     = entry;
            accountId = account.Id;
            isDirty   = true;
        }

        public void Save(AuthContext context)
        {
            if (!isDirty)
                return;

            var model = new AccountGenericUnlockModel
            {
                Id    = accountId,
                Entry = Entry.Id
            };

            context.Add(model);
            isDirty = false;
        }
    }
}

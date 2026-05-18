using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Game.Abstract.Account;
using NexusForever.Game.Abstract.Account.Unlock;
using NexusForever.Game.Account.Costume;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Account.Unlock
{
    public class GenericUnlock : IGenericUnlock
    {
        public GenericUnlockEntryEntry Entry { get; }
        public GenericUnlockType Type => (GenericUnlockType)Entry.GenericUnlockTypeEnum;

        private readonly IAccount account;
        private bool isDirty;

        /// <summary>
        /// Create a new <see cref="IGenericUnlock"/> from existing <see cref="AccountGenericUnlockModel"/> database model.
        /// </summary>
        public GenericUnlock(IAccount account, AccountGenericUnlockModel model)
        {
            this.account = account;
            Entry        = GameTableManager.Instance.GenericUnlockEntry.GetEntry(model.Entry);
            isDirty      = false;
        }

        /// <summary>
        /// Create a new <see cref="IGenericUnlock"/> from supplied <see cref="GenericUnlockEntryEntry"/>.
        /// </summary>
        public GenericUnlock(IAccount account, GenericUnlockEntryEntry entry)
        {
            this.account = account;
            Entry        = entry;
            isDirty      = true;
        }

        public void Save(AuthContext context)
        {
            if (!isDirty)
                return;

            var model = new AccountGenericUnlockModel
            {
                Id    = account.Id,
                Entry = Entry.Id
            };

            context.Add(model);
            isDirty = false;
        }
    }
}

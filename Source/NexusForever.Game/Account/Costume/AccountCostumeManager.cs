using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Game.Abstract.Account;
using NexusForever.Game.Abstract.Account.Costume;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Account.Costume;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Game.Account.Costume
{
    public class AccountCostumeManager : IAccountCostumeManager
    {
        private readonly IAccount account;
        private readonly Dictionary<uint, ICostumeUnlock> costumeUnlocks = new();

        /// <summary>
        /// Create a new <see cref="IAccountCostumeManager"/> from existing <see cref="AccountModel"/> database model.
        /// </summary>
        public AccountCostumeManager(IAccount account, AccountModel accountModel)
        {
            this.account = account;

            foreach (AccountCostumeUnlockModel costumeUnlockModel in accountModel.AccountCostumeUnlock)
                costumeUnlocks.Add(costumeUnlockModel.ItemId, new CostumeUnlock(costumeUnlockModel));
        }

        public void Save(AuthContext context)
        {
            foreach (ICostumeUnlock costumeItem in costumeUnlocks.Values)
                costumeItem.Save(context);
        }

        /// <summary>
        /// Returns if costume item has been unlocked.
        /// </summary>
        public bool HasItemUnlock(uint itemId)
        {
            return costumeUnlocks.TryGetValue(itemId, out ICostumeUnlock costumeUnlock) && !costumeUnlock.PendingDelete;
        }

        /// <summary>
        /// Unlock costume item with supplied <see cref="IItem"/>.
        /// </summary>
        public void UnlockItem(IItem item)
        {
            if (item == null)
            {
                SendCostumeItemUnlock(CostumeUnlockResult.InvalidItem);
                return;
            }

            UnlockItem(item.Id);

            // TODO: make item soulbound
        }

        /// <summary>
        /// Unlock costume item with supplied item id.
        /// </summary>
        public void UnlockItem(uint itemId)
        {
            if (costumeUnlocks.TryGetValue(itemId, out ICostumeUnlock costumeUnlock) && !costumeUnlock.PendingDelete)
            {
                SendCostumeItemUnlock(CostumeUnlockResult.AlreadyKnown);
                return;
            }

            if (costumeUnlocks.Count >= GetMaxUnlockItemCount())
            {
                SendCostumeItemUnlock(CostumeUnlockResult.OutOfSpace);
                return;
            }

            if (costumeUnlock != null)
                costumeUnlock.EnqueueDelete(false);
            else
                costumeUnlocks.Add(itemId, new CostumeUnlock(account, itemId));

            SendCostumeItemUnlock(CostumeUnlockResult.UnlockSuccess, itemId);
        }

        private uint GetMaxUnlockItemCount()
        {
            // client defaults to 1000 if entry doesn't exist
            GameFormulaEntry entry = GameTableManager.Instance.GameFormula.GetEntry(1203);
            if (entry == null)
                return 1000u;

            return entry.Dataint0 + (account.EntitlementManager.GetEntitlement(EntitlementType.AdditionalCostumeUnlocks)?.Amount ?? 0u);
        }

        /// <summary>
        /// Forget costume item unlock of supplied item id.
        /// </summary>
        public void ForgetItem(uint itemId)
        {
            Item2Entry itemEntry = GameTableManager.Instance.Item.GetEntry(itemId);
            if (itemEntry == null)
            {
                SendCostumeItemUnlock(CostumeUnlockResult.InvalidItem);
                return;
            }

            if (!costumeUnlocks.TryGetValue(itemId, out ICostumeUnlock costumeUnlock))
            {
                SendCostumeItemUnlock(CostumeUnlockResult.ForgetItemFailed);
                return;
            }

            costumeUnlock.EnqueueDelete(true);
            SendCostumeItemUnlock(CostumeUnlockResult.ForgetItemSuccess, itemId);
        }

        public void SendInitialPackets()
        {
            account.Session.EnqueueMessageEncrypted(new ServerCostumeItemList
            {
                Items = costumeUnlocks.Keys.ToList()
            });
        }

        /// <summary>
        /// Send <see cref="ServerCostumeItemUnlock"/> will supplied <see cref="CostumeUnlockResult"/> and optional item id.
        /// </summary>
        private void SendCostumeItemUnlock(CostumeUnlockResult result, uint itemId = 0u)
        {
            var costumeItemUnlock = new ServerCostumeItemUnlock
            {
                Result = result
            };

            if (itemId != 0u)
                costumeItemUnlock.ItemId = itemId;

            account.Session.EnqueueMessageEncrypted(costumeItemUnlock);
        }
    }
}

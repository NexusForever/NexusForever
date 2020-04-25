using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Account.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using NexusForever.WorldServer.Game.Prerequisite;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.Account
{
    public class AccountInventory : ISaveAuth, IEnumerable<AccountItem>
    {
        private readonly WorldSession session;

        private readonly Dictionary</* id */ ulong, AccountItem> items = new Dictionary<ulong, AccountItem>();
        private readonly Dictionary</* id */ uint, AccountItemCooldown> cooldowns = new Dictionary<uint, AccountItemCooldown>();
        private readonly HashSet<AccountItem> deletedItems = new HashSet<AccountItem>();

        /// <summary>
        /// Create a new <see cref="AccountInventory"/> for this <see cref="WorldSession"/> with a given <see cref="AccountModel"/>.
        /// </summary>
        public AccountInventory(WorldSession session, AccountModel model)
        {
            this.session = session;

            foreach (AccountItemModel accountItemModel in model.AccountItem)
                items.TryAdd(accountItemModel.Id, new AccountItem(accountItemModel));

            foreach (AccountItemCooldownModel cooldown in model.AccountItemCooldown)
                cooldowns.TryAdd(cooldown.CooldownGroupId, new AccountItemCooldown(cooldown));

            if (cooldowns.Keys.Count == GameTableManager.Instance.AccountItemCooldownGroup.Entries.Length) 
                return;

            foreach (AccountItemCooldownGroupEntry cooldownEntry in GameTableManager.Instance.AccountItemCooldownGroup.Entries)
                cooldowns.TryAdd(cooldownEntry.Id, new AccountItemCooldown(session, cooldownEntry.Id));
        }

        /// <summary>
        /// Save all <see cref="AccountItem"/> changes to the database.
        /// </summary>
        public void Save(AuthContext context)
        {
            foreach (AccountItem deletedItem in deletedItems)
                deletedItem.Save(context);

            deletedItems.Clear();

            foreach (AccountItem item in items.Values)
                item.Save(context);

            foreach (AccountItemCooldown cooldown in cooldowns.Values)
                cooldown.Save(context);
        }

        /// <summary>
        /// Create a new <see cref="AccountItem"/> from an <see cref="AccountItemEntry"/>.
        /// </summary>
        public void ItemCreate(AccountItemEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            AccountItem item = new AccountItem(session, entry);
            items.TryAdd(item.Id, item);

            SendAccountItemList();
        }

        /// <summary>
        /// Delete an <see cref="AccountItem"/> given an ID.
        /// </summary>
        public void ItemDelete(ulong id)
        {
            if (!GetItem(id, out AccountItem item))
                throw new ArgumentOutOfRangeException(nameof(id));

            item.EnqueueDelete();
            deletedItems.Add(item);
            items.Remove(id);
        }

        /// <summary>
        /// Returns an <see cref="AccountItem"/> from this <see cref="AccountInventory"/>, if it exists.
        /// </summary>
        public bool GetItem(ulong id, out AccountItem item)
        {
            if (!items.TryGetValue(id, out item))
                item = null;

            return item != null;
        }

        /// <summary>
        /// Binds the <see cref="AccountItem"/> that matches the given ID to the Character. This should only be called by a Client handler.
        /// </summary>
        public void BindItem(ulong id)
        {
            if (id == 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            if (!GetItem(id, out AccountItem item))
                throw new InvalidOperationException($"AccountItem ID {id} not found.");

            // TODO: Add checks for things like In Combat, Is Dead, etc.

            if (item.Entry.EntitlementId != 0u)
            {
                AccountEntitlement entitlement =
                    session.EntitlementManager.GetAccountEntitlement((EntitlementType) item.Entry.EntitlementId);
                if (entitlement != null)
                    if (entitlement.Amount + item.Entry.EntitlementCount > entitlement.Entry.MaxCount)
                    {
                        SendAccountOperationResult(AccountOperation.TakeItem, AccountOperationResult.MaxEntitlementCount);
                        return;
                    }
            }

            if (item.Entry.PrerequisiteId != 0u &&
                !PrerequisiteManager.Instance.Meets(session.Player, item.Entry.PrerequisiteId))
            {
                SendAccountOperationResult(AccountOperation.TakeItem, AccountOperationResult.Prereq);
                return;
            }

            // Handle Set Cooldown
            if (item.Entry.AccountItemCooldownGroupId != 0u)
            {
                if (IsOnCooldown(item.Entry.AccountItemCooldownGroupId))
                {
                    SendAccountOperationResult(AccountOperation.TakeItem, AccountOperationResult.Cooldown);
                    return;
                }

                SetCooldown(item.Entry.AccountItemCooldownGroupId);
            }

            // Send any Item packets
            if ((item.Flags & AccountItemFlag.MultiClaim) == 0)
            {
                SendAccountItemDelete(item.Id);
                item.EnqueueDelete();
                deletedItems.Add(item);
                items.Remove(item.Id);
            }

            // Send Operation Result
            SendAccountOperationResult(AccountOperation.TakeItem, AccountOperationResult.Ok);

            // Claim Items
            item.ClaimItems(session);
        }

        private bool IsOnCooldown(uint cooldownGroupId)
        {
            if (!cooldowns.TryGetValue(cooldownGroupId, out AccountItemCooldown cooldown))
                throw new ArgumentOutOfRangeException(nameof(cooldownGroupId));

            return cooldown.GetRemainingDuration() > 0;
        }

        private void SetCooldown(uint cooldownGroupId)
        {
            if (!cooldowns.TryGetValue(cooldownGroupId, out AccountItemCooldown cooldown))
                throw new ArgumentOutOfRangeException(nameof(cooldownGroupId));

            uint duration = 0;
            switch (cooldownGroupId)
            {
                case 1:
                    duration = 1800;
                    break;
                case 2:
                    duration = 1800;
                    break;
                case 3:
                    duration = 28800;
                    break;
            }

            cooldown.TriggerWithDuration(duration);
            SendAccountItemCooldowns();
        }

        /// <summary>
        /// Sends initial packets to the <see cref="WorldSession"/>.
        /// </summary>
        public void SendCharacterListPackets()
        {
            SendAccountItemList();
            SendAccountItemCooldowns();
        }

        private void SendAccountItemList()
        {
            ServerAccountItems accountItems = new ServerAccountItems();

            foreach (AccountItem item in items.Values)
                accountItems.AccountItems.Add(item.BuildNetworkModel());

            session.EnqueueMessageEncrypted(accountItems);
        }

        private void SendAccountItemDelete(ulong id)
        {
            session.EnqueueMessageEncrypted(new ServerAccountItemDelete
            {
                UserInventoryId = id
            });
        }

        private void SendAccountOperationResult(AccountOperation operation, AccountOperationResult result)
        {
            session.EnqueueMessageEncrypted(new ServerAccountOperationResult
            {
                Operation = operation,
                Result = result
            });
        }

        private void SendAccountItemCooldowns()
        {
            foreach (AccountItemCooldown cooldown in cooldowns.Values)
            {
                if (cooldown.GetRemainingDuration() == 0)
                    continue;

                session.EnqueueMessageEncrypted(cooldown.BuildNetworkModel());
            }
        }

        public IEnumerator<AccountItem> GetEnumerator()
        {
            return items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

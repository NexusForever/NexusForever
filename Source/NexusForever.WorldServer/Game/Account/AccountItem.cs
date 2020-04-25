using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Account.Static;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Game.Account
{
    public class AccountItem : ISaveAuth
    {
        public uint AccountId { get; }
        public ulong Id { get; }
        public AccountItemEntry Entry { get; }
        public AccountItemFlag Flags { get; }

        private AccountItemSaveMask saveMask;

        /// <summary>
        /// Creates an <see cref="AccountItem"/> from a given database model.
        /// </summary>
        public AccountItem(AccountItemModel model)
        {
            AccountId = model.AccountId;
            Id = model.Id;

            Entry = GameTableManager.Instance.AccountItem.GetEntry(model.ItemId);
            if (Entry == null)
                throw new InvalidOperationException($"AccountItem Entry {model.ItemId} cannot be found.");

            Flags = (AccountItemFlag)Entry.Flags;

            saveMask |= AccountItemSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="AccountItem"/> from a <see cref="AccountItemEntry"/>.
        /// </summary>
        public AccountItem(WorldSession session, AccountItemEntry entry)
        {
            AccountId = session.Account.Id;
            Id = AssetManager.Instance.NextAccountItemId;
            Entry = entry ?? throw new ArgumentNullException(nameof(entry));
            Flags = (AccountItemFlag)entry.Flags;

            saveMask |= AccountItemSaveMask.Create;
        }

        /// <summary>
        /// Saves this <see cref="AccountItem"/> to the database.
        /// </summary>
        public void Save(AuthContext context)
        {
            if (saveMask == AccountItemSaveMask.None)
                return;

            if ((saveMask & AccountItemSaveMask.Create) != 0)
            {
                var model = new AccountItemModel
                {
                    Id = Id,
                    AccountId = AccountId,
                    ItemId = Entry.Id
                };

                context.Add(model);
            }
            else if ((saveMask & AccountItemSaveMask.Delete) != 0)
            {
                var model = new AccountItemModel
                {
                    Id = Id,
                    AccountId = AccountId
                };

                context.Entry(model).State = EntityState.Deleted;
            }

            saveMask = AccountItemSaveMask.None;
        }

        /// <summary>
        /// Queues this <see cref="AccountItem"/> for deletion.
        /// </summary>
        public void EnqueueDelete()
        {
            saveMask = (saveMask & AccountItemSaveMask.Create) != 0 ? AccountItemSaveMask.None : AccountItemSaveMask.Delete;
        }

        /// <summary>
        /// Return a <see cref="AccountInventoryItem"/> that describes this <see cref="AccountItem"/> to be sent to the client.
        /// </summary>
        public AccountInventoryItem BuildNetworkModel()
        {
            return new AccountInventoryItem
            {
                Id     = Id,
                ItemId = Entry.Id
            };
        }

        /// <summary>
        /// Sends all items as part of this <see cref="AccountItem"/> to the requesting <see cref="WorldSession"/>.
        /// </summary>
        public void ClaimItems(WorldSession session)
        {
            if (session.Account.Id != AccountId)
                throw new ArgumentException(nameof(session));

            if (Entry.Item2Id != 0u)
            {
                List<uint> attachmentList = new List<uint>();
                for (int i = 0; i < Entry.EntitlementCount; i++)
                    attachmentList.Add(Entry.Item2Id);

                // Couple items have 0 for entitlementCount, though most have 1+.
                if (attachmentList.Count == 0u)
                    attachmentList.Add(Entry.Item2Id);

                session.Player.MailManager.SendMail(26454, Mail.Static.DeliveryTime.Instant, 461265, 461266, attachmentList, automaticClaim: true);
            }
                

            if (Entry.EntitlementId != 0u)
                session.EntitlementManager.SetAccountEntitlement((EntitlementType)Entry.EntitlementId, (int)Entry.EntitlementCount);

            if (Entry.AccountCurrencyEnum != 0u)
                session.AccountCurrencyManager.CurrencyAddAmount((AccountCurrencyType)Entry.AccountCurrencyEnum, Entry.AccountCurrencyAmount);
        }
    }
}

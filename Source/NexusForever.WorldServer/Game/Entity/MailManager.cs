using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared;
using NexusForever.Shared.Database;
using NexusForever.Shared.Game;
using NexusForever.Shared.Game.Events;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Mail;
using NexusForever.WorldServer.Game.Mail.Static;
using NexusForever.WorldServer.Game.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Game.Entity
{
    public class MailManager : IUpdate, ISaveCharacter
    {
        private readonly Player player;
        private readonly Queue<MailItem> outgoingMail = new Queue<MailItem>();
        private readonly List<MailItem> pendingMail = new List<MailItem>();
        private readonly Dictionary<ulong, MailItem> availableMail = new Dictionary<ulong, MailItem>();

        // timer to check pending mail ever second
        private readonly UpdateTimer mailTimer = new UpdateTimer(1000d);

        /// <summary>
        /// Create a new <see cref="MailManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public MailManager(Player owner, CharacterModel model)
        {
            player = owner;
            foreach (CharacterMailModel mailModel in model.Mail)
            {
                var mail = new MailItem(mailModel);
                if (mail.IsReadyToDeliver())
                    availableMail.Add(mail.Id, mail);
                else
                    pendingMail.Add(mail);
            }
        }

        public void Update(double lastTick)
        {
            mailTimer.Update(lastTick);
            if (mailTimer.HasElapsed)
            {
                bool sendAvailableMail = false;
                foreach (MailItem mail in pendingMail)
                {
                    if (!mail.IsReadyToDeliver())
                        continue;

                    availableMail.Add(mail.Id, mail);
                    sendAvailableMail = true;
                }

                // prevent sending multiple mail packets
                if (sendAvailableMail)
                    SendAvailableMail();

                // TODO: remove expired mail

                mailTimer.Reset();
            }
        }

        public void Save(CharacterContext context)
        {
            while (outgoingMail.TryDequeue(out MailItem mail))
            {
                mail.Save(context);

                MailManager mailManager;
                if (mail.RecipientId == player.CharacterId)
                    mailManager = player.MailManager;
                else
                {
                    // ReSharper disable once AccessToModifiedClosure
                    WorldSession session = NetworkManager<WorldSession>.Instance.GetSession(c => c.Player.CharacterId == mail.RecipientId);
                    mailManager = session?.Player.MailManager;
                }

                // deliver mail if user is online
                mailManager?.EnqueueMail(mail);
            }

            foreach (MailItem mail in availableMail.Values.ToList())
            {
                if (mail.PendingDelete)
                    availableMail.Remove(mail.Id);

                mail.Save(context);
            }
        }

        /// <summary>
        /// Called by <see cref="Player"/> to send available mail on entering map.
        /// </summary>
        public void SendInitialPackets()
        {
            SendAvailableMail();
        }

        private void SendAvailableMail()
        {
            var mailAvailable = new ServerMailAvailable
            {
                NewMail = true
            };

            foreach (MailItem mail in availableMail.Values.Where(m => !m.PendingDelete))
                mailAvailable.MailList.Add(BuildNetworkMail(mail));

            player.Session.EnqueueMessageEncrypted(mailAvailable);
        }

        /// <summary>
        /// Handles converting <see cref="MailItem"/> to <see cref="ServerMailAvailable.Mail"/> for the client to process
        /// </summary>
        private static ServerMailAvailable.Mail BuildNetworkMail(MailItem mail)
        {
            bool isPlayer = mail.SenderType == SenderType.Player || mail.SenderType == SenderType.GM;

            var serverMailItem = new ServerMailAvailable.Mail
            {
                MailId               = mail.Id,
                SenderType           = mail.SenderType,
                Subject              = mail.Subject,
                Message              = mail.Message,
                TextEntrySubject     = mail.TextEntrySubject,
                TextEntryMessage     = mail.TextEntryMessage,
                CreatureId           = !isPlayer ? mail.CreatureId : 0,
                CurrencyGiftType     = 0,
                CurrencyGiftAmount   = !mail.IsCashOnDelivery && !mail.HasPaidOrCollectedCurrency ? mail.CurrencyAmount : 0,
                CostOnDeliveryAmount = mail.IsCashOnDelivery && !mail.HasPaidOrCollectedCurrency ? mail.CurrencyAmount : 0,
                ExpiryTimeInDays     = mail.ExpiryTime,
                Flags                = mail.Flags,
                Sender = new TargetPlayerIdentity
                {
                    RealmId     = isPlayer ? WorldServer.RealmId : (ushort)0,
                    CharacterId = isPlayer ? mail.SenderId : 0ul
                },
            };

            foreach (MailAttachment attachment in mail)
                serverMailItem.Attachments.Add(BuildNetworkMailAttachment(attachment));

            return serverMailItem;
        }

        /// <summary>
        /// Handles converting <see cref="MailAttachment"/> to <see cref="ServerMailAvailable.Attachment"/> for the client to process
        /// </summary>
        private static ServerMailAvailable.Attachment BuildNetworkMailAttachment(MailAttachment attachment)
        {
            var serverMailAttachment = new ServerMailAvailable.Attachment
            {
                ItemId = attachment.Item.Id,
                Amount = attachment.Item.StackCount
            };

            return serverMailAttachment;
        }

        /// <summary>
        /// Enqueue new incoming <see cref="MailItem"/> to be processed.
        /// </summary>
        public void EnqueueMail(MailItem mail)
        {
            if (mail.IsReadyToDeliver())
                availableMail.Add(mail.Id, mail);
            else
                pendingMail.Add(mail);
        }

        /// <summary>
        /// Send mail to another <see cref="Player"/>.
        /// </summary>
        public void SendMail(ClientMailSend mailSend)
        {
            player.Session.EnqueueEvent(new TaskGenericEvent<CharacterModel>(DatabaseManager.Instance.CharacterDatabase.GetCharacterByName(mailSend.Name),
                targetCharacter =>
            {
                var items = new List<Item>();
                GenericError GetResult()
                {
                    if (targetCharacter == null)
                        return GenericError.MailCannotFindPlayer;

                    if (targetCharacter.Id == player.CharacterId)
                        return GenericError.MailCannotMailSelf;

                    // TODO: Check that the player is not blocked

                    if (mailSend.CreditsRequested > 0ul && mailSend.CreditsSent > 0ul)
                        return GenericError.MailCanNotHaveCoDAndGift;

                    if (mailSend.CreditsRequested > 0ul && mailSend.Items.All(i => i == 0ul))
                        return GenericError.MailFailedToCreate;

                    if (mailSend.Items.Any(i => i != 0ul))
                    {
                        if (!IsTargetMailBoxInRange(mailSend.UnitId))
                            return GenericError.MailMailBoxOutOfRange;

                        foreach (ulong itemGuid in mailSend.Items.Where(i => i != 0ul))
                        {
                            Item item = player.Inventory.GetItem(itemGuid);
                            if (item == null)
                                return GenericError.MailInvalidInventorySlot;

                            // TODO: Check the Item can be traded.
                            items.Add(item);
                        }
                    }

                    uint cost = CalculateMailCost(mailSend.DeliveryTime, items);
                    if (!player.CurrencyManager.CanAfford(CurrencyType.Credits, cost))
                        return GenericError.MailInsufficientFunds;

                    if (!player.CurrencyManager.CanAfford(CurrencyType.Credits, mailSend.CreditsSent))
                        return GenericError.MailInsufficientFunds;

                    return GenericError.Ok;
                }

                GenericError result = GetResult();
                if (result == GenericError.Ok)
                {
                    var parameters = new MailParameters
                    {
                        RecipientCharacterId = targetCharacter.Id,
                        SenderCharacterId    = player.CharacterId,
                        MessageType          = SenderType.Player,
                        Subject              = mailSend.Subject,
                        Body                 = mailSend.Message,
                        MoneyToGive          = mailSend.CreditsSent,
                        CodAmount            = mailSend.CreditsRequested,
                        DeliveryTime         = mailSend.DeliveryTime
                    };

                    foreach (Item item in items)
                        player.Inventory.ItemRemove(item);

                    SendMail(parameters, items);

                    uint cost = CalculateMailCost(mailSend.DeliveryTime, items);
                    player.CurrencyManager.CurrencySubtractAmount(CurrencyType.Credits, cost);

                    if (mailSend.CreditsSent > 0ul)
                        player.CurrencyManager.CurrencySubtractAmount(CurrencyType.Credits, mailSend.CreditsSent);

                }

                player.Session.EnqueueMessageEncrypted(new ServerMailResult
                {
                    Action = 1,
                    MailId = 0,
                    Result = result
                });
            }));
        }

        /// <summary>
        /// Send mail to self from a creature.
        /// </summary>
        public void SendMail(uint creatureId, DeliveryTime time, uint subject, uint body, IEnumerable<uint> itemIds)
        {
            if (GameTableManager.Instance.Creature2.GetEntry(creatureId) == null)
                throw new ArgumentException($"Invalid creature {creatureId} for mail sender!");

            if (GameTableManager.Instance.LocalizedText.GetEntry(subject) == null)
                throw new ArgumentException($"Invalid localised text {subject} for mail subject!");

            if (GameTableManager.Instance.LocalizedText.GetEntry(body) == null)
                throw new ArgumentException($"Invalid localised text {body} for mail body!");

            var parameters = new MailParameters
            {
                MessageType          = SenderType.Creature,
                RecipientCharacterId = player.CharacterId,
                CreatureId           = creatureId,
                SubjectStringId      = subject,
                BodyStringId         = body,
                DeliveryTime         = time
            };

            var items = new List<Item>();
            foreach (uint itemId in itemIds)
            {
                Item2Entry itemEntry = GameTableManager.Instance.Item.GetEntry(itemId);
                if (itemEntry == null)
                    throw new ArgumentException($"Invalid item {itemId} for mail attachment!");

                var item = new Item(null, itemEntry);
                items.Add(item);
            }

            SendMail(parameters, items);
        }

        // TODO: Handle sending mail from auctions to users upon auction end
        // TODO: Handle sending mail from GMs to replace missing items

        private void SendMail(MailParameters parameters, IEnumerable<Item> items)
        {
            var mail = new MailItem(parameters);

            uint index = 0;
            foreach (Item item in items)
            {
                var attachment = new MailAttachment(mail.Id, index++, item);
                mail.AttachmentAdd(attachment);
            }

            // NOTE: outgoing mail is flushed on character save to prevent any issues,
            // this means that instant mail could take up to 60 seconds (by default) to actually arrive
            outgoingMail.Enqueue(mail);
        }

        private uint CalculateMailCost(DeliveryTime time, List<Item> items)
        {
            GameFormulaEntry GetMailParameters()
            {
                if (items.Count == 0)
                    return GameTableManager.Instance.GameFormula.GetEntry(860);

                return time switch
                {
                    DeliveryTime.Instant => GameTableManager.Instance.GameFormula.GetEntry(861),
                    DeliveryTime.Hour    => GameTableManager.Instance.GameFormula.GetEntry(862),
                    DeliveryTime.Day     => GameTableManager.Instance.GameFormula.GetEntry(863),
                    _                    => null
                };
            }

            GameFormulaEntry parameters = GetMailParameters();
            uint cost = parameters.Dataint0;

            foreach (Item item in items)
            {
                cost += parameters.Dataint01;
                // only instant delivery speed takes item worth into consideration
                if (parameters.Datafloat01 > 0.0f)
                {
                    if (item.GetVendorSellCurrency(0) == CurrencyType.Credits)
                        cost += (uint)(item.GetVendorSellAmount(0) * parameters.Datafloat01);
                    if (item.GetVendorSellCurrency(1) == CurrencyType.Credits)
                        cost += (uint)(item.GetVendorSellAmount(1) * parameters.Datafloat01);
                }
            }

            return cost;
        }

        /// <summary>
        /// Delete a <see cref="MailItem"/> with supplied id.
        /// </summary>
        public void MailDelete(ulong mailId)
        {
            GenericError result = GenericError.Ok;
            if (!availableMail.TryGetValue(mailId, out MailItem mailItem))
                result = GenericError.MailDoesNotExist;

            if (result == GenericError.Ok)
            {
                // TODO: Confirm that this user is allowed to delete this mail
                mailItem.EnqueueDelete();

                player.Session.EnqueueMessageEncrypted(new ServerMailUnavailable
                {
                    MailId = mailItem.Id
                });
            }

            player.Session.EnqueueMessageEncrypted(new ServerMailResult
            {
                Action = 5,
                MailId = mailId,
                Result = result
            });
        }

        /// <summary>
        /// Mark a <see cref="MailItem"/> as read with supplied id.
        /// </summary>
        public void MailMarkAsRead(ulong mailId)
        {
            if (!availableMail.TryGetValue(mailId, out MailItem mailItem))
                return;

            mailItem.MarkAsRead();
        }

        /// <summary>
        /// Pay cash on delivery for a <see cref="MailItem"/> with supplied id.
        /// </summary>
        public void MailPayCod(ulong mailId)
        {
            MailItem mail;
            GenericError GetResult()
            {
                if (!availableMail.TryGetValue(mailId, out mail))
                    return GenericError.MailDoesNotExist;

                if (!player.CurrencyManager.CanAfford(CurrencyType.Credits, mail.CurrencyAmount))
                    return GenericError.MailInsufficientFunds;

                if (mail.HasPaidOrCollectedCurrency)
                    return GenericError.MailBusy;

                return GenericError.Ok;
            }

            GenericError result = GetResult();
            if (result == GenericError.Ok)
            {
                player.CurrencyManager.CurrencySubtractAmount(CurrencyType.Credits, mail.CurrencyAmount);
                mail.PayOrTakeCash();

                var parameters = new MailParameters
                {
                    RecipientCharacterId = mail.SenderId,
                    SenderCharacterId    = mail.RecipientId,
                    MessageType          = SenderType.Player,
                    Subject              = $"Cash from: {mail.Subject}",
                    MoneyToGive          = mail.CurrencyAmount,
                    DeliveryTime         = DeliveryTime.Instant
                };

                SendMail(parameters, Enumerable.Empty<Item>());
            }

            player.Session.EnqueueMessageEncrypted(new ServerMailResult
            {
                Action = 3,
                MailId = mailId,
                Result = result
            });
        }

        /// <summary>
        /// Return <see cref="MailItem"/> to original sender with supplied id.
        /// </summary>
        public void ReturnMail(ulong mailId)
        {
            MailItem mailItem;
            GenericError GetResult()
            {
                if (!availableMail.TryGetValue(mailId, out mailItem))
                    return GenericError.MailDoesNotExist;

                if ((mailItem.Flags & MailFlag.NotReturnable) != 0)
                    return GenericError.MailCannotReturn;

                return GenericError.Ok;
            }

            GenericError result = GetResult();
            if (result == GenericError.Ok)
            {
                mailItem.ReturnMail();

                availableMail.Remove(mailItem.Id);
                outgoingMail.Enqueue(mailItem);

                player.Session.EnqueueMessageEncrypted(new ServerMailUnavailable
                {
                    MailId = mailId
                });
            }

            player.Session.EnqueueMessageEncrypted(new ServerMailResult
            {
                Action = 1,
                MailId = mailId,
                Result = result
            });
        }

        /// <summary>
        /// Take attachment from <see cref="MailItem"/> with supplied id and index.
        /// </summary>
        public void MailTakeAttachment(ulong mailId, uint attachmentIndex, uint unitId)
        {
            MailItem mailItem;
            MailAttachment mailAttachment = null;

            GenericError GetResult()
            {
                if (!availableMail.TryGetValue(mailId, out mailItem))
                    return GenericError.MailDoesNotExist;

                mailAttachment = mailItem.GetAttachment(attachmentIndex);
                if (mailAttachment == null)
                    return GenericError.MailNoAttachment;

                if (unitId == 0u || !IsTargetMailBoxInRange(unitId))
                    return GenericError.MailMailBoxOutOfRange;

                if (player.Inventory.IsInventoryFull())
                    return GenericError.ItemInventoryFull;

                return GenericError.Ok;
            }

            GenericError result = GetResult();
            if (result == GenericError.Ok)
            {
                mailAttachment.Item.CharacterId = player.CharacterId;
                player.Inventory.AddItem(mailAttachment.Item, InventoryLocation.Inventory);

                mailItem.MarkAsNotReturnable();
                mailItem.AttachmentDelete(mailAttachment, attachmentIndex);
            }

            player.Session.EnqueueMessageEncrypted(new ServerMailTakeAttachment
            {
                MailId = mailId,
                Result = result,
                Index  = attachmentIndex
            });
        }

        /// <summary>
        /// Take cash from <see cref="MailItem"/> with supplied id.
        /// </summary>
        public void MailTakeCash(ulong mailId, uint unitId)
        {
            MailItem mailItem;
            GenericError GetResult()
            {
                if (!availableMail.TryGetValue(mailId, out mailItem))
                    return GenericError.MailDoesNotExist;

                // probably not the correct error
                if (mailItem.HasPaidOrCollectedCurrency)
                    return GenericError.MailNoAttachment;

                if (unitId == 0u || !IsTargetMailBoxInRange(unitId))
                    return GenericError.MailMailBoxOutOfRange;

                return GenericError.Ok;
            }

            GenericError result = GetResult();
            if (result == GenericError.Ok)
            {
                player.CurrencyManager.CurrencyAddAmount(mailItem.CurrencyType, mailItem.CurrencyAmount);
                mailItem.PayOrTakeCash();
            }

            player.Session.EnqueueMessageEncrypted(new ServerMailResult
            {
                Action = 2,
                MailId = mailId,
                Result = result
            });
        }

        /// <summary>
        /// Checks to see if the targeted <see cref="Mailbox"/> is in range.
        /// </summary>
        private bool IsTargetMailBoxInRange(uint unitId)
        {
            // native client function MailSystemLib.AtMailbox also uses entry 237 for distance check
            GameFormulaEntry entry = GameTableManager.Instance.GameFormula.GetEntry(237);
            if (entry == null)
                throw new InvalidOperationException();

            var entity = player.GetVisible<WorldEntity>(unitId);
            return entity is Mailbox && Vector3.Distance(player.Position, entity.Position) < entry.Datafloat0;
        }
    }
}

using System.Numerics;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Character;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Mail;
using NexusForever.Game.Character;
using NexusForever.Game.Mail;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Mail;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;
using NexusForever.Shared.Game;

namespace NexusForever.Game.Entity
{
    public class MailManager : IMailManager
    {
        private readonly IPlayer player;
        private readonly Queue<IMailItem> outgoingMail = new();
        private readonly List<IMailItem> pendingMail = new();
        private readonly Dictionary<ulong, IMailItem> availableMail = new();

        // timer to check pending mail ever second
        private readonly UpdateTimer mailTimer = new(1000d);

        /// <summary>
        /// Create a new <see cref="IMailManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public MailManager(IPlayer owner, CharacterModel model)
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
                foreach (IMailItem mail in pendingMail)
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
            while (outgoingMail.TryDequeue(out IMailItem mail))
            {
                mail.Save(context);

                if (mail.RecipientId == player.CharacterId)
                    player.MailManager.EnqueueMail(mail);
                else
                {
                    IPlayer player = PlayerManager.Instance.GetPlayer(mail.RecipientId);
                    player?.MailManager.EnqueueMail(mail);
                }
            }

            foreach (IMailItem mail in availableMail.Values.ToList())
            {
                if (mail.PendingDelete)
                    availableMail.Remove(mail.Id);

                mail.Save(context);
            }
        }

        /// <summary>
        /// Called by <see cref="IPlayer"/> to send available mail on entering map.
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

            foreach (IMailItem mail in availableMail.Values.Where(m => !m.PendingDelete))
                mailAvailable.MailList.Add(mail.Build());

            player.Session.EnqueueMessageEncrypted(mailAvailable);
        }

        /// <summary>
        /// Enqueue new incoming <see cref="IMailItem"/> to be processed.
        /// </summary>
        public void EnqueueMail(IMailItem mail)
        {
            if (mail.IsReadyToDeliver())
                availableMail.Add(mail.Id, mail);
            else
                pendingMail.Add(mail);
        }

        /// <summary>
        /// Send mail to another <see cref="IPlayer"/>.
        /// </summary>
        public void SendMail(ClientMailSend mailSend)
        {
            ICharacter targetCharacter = CharacterManager.Instance.GetCharacter(mailSend.Name);

            var items = new List<IItem>();
            GenericError GetResult()
            {
                if (targetCharacter == null)
                    return GenericError.MailCannotFindPlayer;

                if (targetCharacter.CharacterId == player.CharacterId)
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
                        IItem item = player.Inventory.GetItem(itemGuid);
                        if (item == null)
                            return GenericError.MailInvalidInventorySlot;

                        if (item.Location == InventoryLocation.Equipped)
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
                    RecipientCharacterId = targetCharacter.CharacterId,
                    SenderCharacterId    = player.CharacterId,
                    MessageType          = SenderType.Player,
                    Subject              = mailSend.Subject,
                    Body                 = mailSend.Message,
                    MoneyToGive          = mailSend.CreditsSent,
                    CodAmount            = mailSend.CreditsRequested,
                    DeliveryTime         = mailSend.DeliveryTime
                };

                foreach (IItem item in items)
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

            var items = new List<IItem>();
            foreach (uint itemId in itemIds)
            {
                IItemInfo info = ItemManager.Instance.GetItemInfo(itemId);
                if (info == null)
                    throw new ArgumentException($"Invalid item {itemId} for mail attachment!");

                var item = new Item(null, info);
                items.Add(item);
            }

            SendMail(parameters, items);
        }

        // TODO: Handle sending mail from auctions to users upon auction end
        // TODO: Handle sending mail from GMs to replace missing items
        private void SendMail(MailParameters parameters, IEnumerable<IItem> items)
        {
            var mail = new MailItem(parameters);

            uint index = 0;
            foreach (IItem item in items)
            {
                var attachment = new MailAttachment(mail.Id, index++, item);
                mail.AttachmentAdd(attachment);
            }

            // NOTE: outgoing mail is flushed on character save to prevent any issues,
            // this means that instant mail could take up to 60 seconds (by default) to actually arrive
            outgoingMail.Enqueue(mail);
        }

        private uint CalculateMailCost(DeliveryTime time, List<IItem> items)
        {
            GameFormulaEntry GetMailParameters()
            {
                if (items.Count == 0)
                    return GameTableManager.Instance.GameFormula.GetEntry(860);

                return time switch
                {
                    DeliveryTime.Instant => GameTableManager.Instance.GameFormula.GetEntry(861),
                    DeliveryTime.Hour => GameTableManager.Instance.GameFormula.GetEntry(862),
                    DeliveryTime.Day => GameTableManager.Instance.GameFormula.GetEntry(863),
                    _ => null
                };
            }

            GameFormulaEntry parameters = GetMailParameters();
            uint cost = parameters.Dataint0;

            foreach (IItem item in items)
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
        /// Delete a <see cref="IMailItem"/> with supplied id.
        /// </summary>
        public void MailDelete(ulong mailId)
        {
            GenericError result = GenericError.Ok;
            if (!availableMail.TryGetValue(mailId, out IMailItem mailItem))
                result = GenericError.MailDoesNotExist;

            if (result == GenericError.Ok)
            {
                // TODO: Confirm that this user is allowed to delete this mail
                mailItem.EnqueueDelete(true);

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
        /// Mark a <see cref="IMailItem"/> as read with supplied id.
        /// </summary>
        public void MailMarkAsRead(ulong mailId)
        {
            if (!availableMail.TryGetValue(mailId, out IMailItem mailItem))
                return;

            mailItem.MarkAsRead();
        }

        /// <summary>
        /// Pay cash on delivery for a <see cref="IMailItem"/> with supplied id.
        /// </summary>
        public void MailPayCod(ulong mailId)
        {
            IMailItem mail;
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
        /// Return <see cref="IMailItem"/> to original sender with supplied id.
        /// </summary>
        public void ReturnMail(ulong mailId)
        {
            IMailItem mailItem;
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
        /// Take attachment from <see cref="IMailItem"/> with supplied id and index.
        /// </summary>
        public void MailTakeAttachment(ulong mailId, uint attachmentIndex, uint unitId)
        {
            IMailItem mailItem;
            IMailAttachment mailAttachment = null;

            GenericError GetResult()
            {
                if (!availableMail.TryGetValue(mailId, out mailItem))
                    return GenericError.MailDoesNotExist;

                mailAttachment = mailItem.GetAttachment(attachmentIndex);
                if (mailAttachment == null)
                    return GenericError.MailNoAttachment;

                if (unitId == 0u || !IsTargetMailBoxInRange(unitId))
                    return GenericError.MailMailBoxOutOfRange;

                if (player.Inventory.IsInventoryFull(InventoryLocation.Inventory))
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
        /// Take cash from <see cref="IMailItem"/> with supplied id.
        /// </summary>
        public void MailTakeCash(ulong mailId, uint unitId)
        {
            IMailItem mailItem;
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
        /// Checks to see if the targeted <see cref="IMailItem"/> is in range.
        /// </summary>
        private bool IsTargetMailBoxInRange(uint unitId)
        {
            // native client function MailSystemLib.AtMailbox also uses entry 237 for distance check
            GameFormulaEntry entry = GameTableManager.Instance.GameFormula.GetEntry(237);
            if (entry == null)
                throw new InvalidOperationException();

            var entity = player.GetVisible<IWorldEntity>(unitId);
            return entity is IMailboxEntity && Vector3.Distance(player.Position, entity.Position) < entry.Datafloat0;
        }
    }
}

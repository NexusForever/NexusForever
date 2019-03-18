using NexusForever.Shared.Game.Events;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Database.Character;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Mail;
using NexusForever.WorldServer.Game.Mail.Static;
using NexusForever.WorldServer.Game.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    class MailHandler
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Handles deletion of a <see cref="MailItem"/>
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientMailDelete)]
        public static void HandleMailDelete(WorldSession session, ClientMailDelete clientMailDelete)
        {
            foreach(ulong mailId in clientMailDelete.MailList)
                if (session.Player.AvailableMail.TryGetValue(mailId, out MailItem mailItem))
                {
                    // TODO: Confirm that this user is allowed to delete this mail
                    mailItem.EnqueueDelete();

                    session.EnqueueMessageEncrypted(new ServerMailResult
                    {
                        Action = 5,
                        MailId = mailItem.Id,
                        Result = 0
                    });
                    session.EnqueueMessageEncrypted(new ServerMailUnavailable
                    {
                        MailId = mailItem.Id
                    });
                }
        }

        /// <summary>
        /// Handles the client opening a new <see cref="MailItem"/>
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientMailOpen)]
        public static void HandleMailOpen(WorldSession session, ClientMailOpen clientMailOpen)
        {
            foreach (ulong mailId in clientMailOpen.MailList)
                if (session.Player.AvailableMail.TryGetValue(mailId, out MailItem mailItem))
                    mailItem.MarkAsRead();
        }

        /// <summary>
        /// Handles the client request to pay the credits requested for a <see cref="MailItem"/>
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientMailPayCOD)]
        public static void HandleMailPayCOD(WorldSession session, ClientMailPayCOD clientMailPayCOD)
        {
            GenericError result = GenericError.Ok;
            if (session.Player.AvailableMail.TryGetValue(clientMailPayCOD.MailId, out MailItem mailItem))
            {
                if (!session.Player.CurrencyManager.CanAfford(1, mailItem.CurrencyAmount))
                    result = GenericError.Mail_InsufficientFunds;

                if (mailItem.HasPaidOrCollectedCurrency)
                    result = GenericError.Mail_Busy;

                if (result == GenericError.Ok)
                {
                    session.Player.CurrencyManager.CurrencySubtractAmount(1, mailItem.CurrencyAmount);
                    MailManager.PayCOD(session, mailItem);
                }
            }
            else
                result = GenericError.Mail_Squelched;

            session.EnqueueMessageEncrypted(new ServerMailResult
            {
                Action = 3,
                MailId = mailItem.Id,
                Result = result
            });
        }

        /// <summary>
        /// Handles returning a <see cref="MailItem"/> to its original sender
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientMailReturn)]
        public static void HandleMailReturn(WorldSession session, ClientMailReturn clientMailReturn)
        {
            GenericError result = GenericError.Ok;

            if(session.Player.AvailableMail.TryGetValue(clientMailReturn.MailId, out MailItem mailItem))
            {
                if ((mailItem.Flags & MailFlag.NotReturnable) != 0)
                    result = GenericError.Mail_CannotReturn;

                if (result == GenericError.Ok)
                {
                    MailManager.ReturnMail(session, mailItem);
                    session.EnqueueMessageEncrypted(new ServerMailUnavailable
                    {
                        MailId = clientMailReturn.MailId
                    });
                }
            }
            session.EnqueueMessageEncrypted(new ServerMailResult
            {
                Action = 1,
                MailId = clientMailReturn.MailId,
                Result = result
            });
        }

        // TODO: Implement ClientMailTakeAllFromSelection

        /// <summary>
        /// Handles a client request to retrieve a <see cref="MailAttachment"/> from a <see cref="MailItem"/>
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientMailTakeAttachment)]
        public static void HandleMailTakeAttackment(WorldSession session, ClientMailTakeAttachment clientMailTakeAttachment)
        {
            GenericError result = GenericError.Ok;

            if (clientMailTakeAttachment.UnitId <= 0 || !MailManager.IsTargetMailBoxInRange(session, clientMailTakeAttachment.UnitId))
                result = GenericError.Mail_MailBoxOutOfRange;

            if (session.Player.Inventory.IsInventoryFull())
                result = GenericError.Item_InventoryFull; // Handle when inventory is empty

            if (result == GenericError.Ok)
            {
                session.Player.AvailableMail.TryGetValue(clientMailTakeAttachment.MailId, out MailItem mailItem);
                if (mailItem != null)
                {
                    MailAttachment mailAttachment = mailItem.GetAttachment((int)clientMailTakeAttachment.Index);

                    // TODO: Check Inventory Full before proceeding
                    mailAttachment.Item.CharacterId = session.Player.CharacterId;
                    session.Player.Inventory.AddItem(mailAttachment.Item, Game.Entity.Static.InventoryLocation.Inventory);

                    mailItem.MarkAsNotReturnable();
                    mailItem.AttachmentDelete(mailAttachment, (int)clientMailTakeAttachment.Index);
                }
                else
                    result = GenericError.Mail_InvalidInventorySlot;
            }

            session.EnqueueMessageEncrypted(new ServerMailTakeAttachment
            {
                MailId = clientMailTakeAttachment.MailId,
                Result = result,
                Index = clientMailTakeAttachment.Index
            });
        }

        /// <summary>
        /// Handles a client request to retrieve the currency sent with a <see cref="MailItem"/>
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientMailTakeCash)]
        public static void HandleMailTakeCash(WorldSession session, ClientMailTakeCash clientMailTakeCash)
        {
            GenericError result = GenericError.Ok;

            if (clientMailTakeCash.UnitId <= 0 || !MailManager.IsTargetMailBoxInRange(session, clientMailTakeCash.UnitId))
                result = GenericError.Mail_MailBoxOutOfRange;

            if (result == GenericError.Ok)
            {
                MailItem mailItem = session.Player.AvailableMail[clientMailTakeCash.MailId];
                if (mailItem != null)
                {
                    session.Player.CurrencyManager.CurrencyAddAmount((byte)mailItem.CurrencyType, mailItem.CurrencyAmount);
                    mailItem.PayOrTakeCash();
                }
            }

            session.EnqueueMessageEncrypted(new ServerMailResult
            {
                Action = 2,
                MailId = clientMailTakeCash.MailId,
                Result = result
            });
        }

        /// <summary>
        /// Handles a client request to send a mail to another player
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientMailSend)]
        public static void HandleMailSend(WorldSession session, ClientMailSend clientMailSend)
        {
            GenericError result = GenericError.Ok;

            session.EnqueueEvent(new TaskGenericEvent<Character>(CharacterDatabase.GetCharacterByName(clientMailSend.Name),
                targetCharacter =>
            {
                if (targetCharacter != null)
                {
                    if (!session.Player.CurrencyManager.CanAfford(1, clientMailSend.CreditsSent))
                        result = GenericError.Mail_InsufficientFunds;

                    // TODO: Check that the player is not blocked

                    if (clientMailSend.CreditsRequested > 0 && clientMailSend.CreditsSent > 0)
                        result = GenericError.Mail_CanNotHaveCoDAndGift;

                    if ((clientMailSend.Items.Count > 0 && clientMailSend.Items[0] > 0) && !MailManager.IsTargetMailBoxInRange(session, clientMailSend.UnitId))
                        result = GenericError.Mail_FailedToCreate;

                    if (clientMailSend.CreditsRequested > 0 && clientMailSend.Items.Count <= 0)
                        result = GenericError.Mail_FailedToCreate;

                    if (result == GenericError.Ok)
                    {
                        MailManager.SendMailToPlayer(session, clientMailSend, targetCharacter, out MailItem newMail);
                        if (newMail == null)
                            result = GenericError.Mail_FailedToCreate;
                    }
                }
                else
                    result = GenericError.Mail_CannotFindPlayer;

                session.EnqueueMessageEncrypted(new ServerMailResult
                {
                    Action = 1,
                    MailId = 0,
                    Result = result
                });
            }));
        }
    }
}

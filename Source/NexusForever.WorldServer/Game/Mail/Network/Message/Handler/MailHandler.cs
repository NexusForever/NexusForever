using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;

namespace NexusForever.WorldServer.Game.Mail.Network.Message.Handler
{
    class MailHandler
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

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

        [MessageHandler(GameMessageOpcode.ClientMailOpen)]
        public static void HandleMailOpen(WorldSession session, ClientMailOpen clientMailOpen)
        {
            foreach (ulong mailId in clientMailOpen.MailList)
                if (session.Player.AvailableMail.TryGetValue(mailId, out MailItem mailItem))
                    mailItem.MarkAsRead();
        }

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
                    session.Player.CurrencyManager.CurrencySubtractAmount(1, mailItem.CurrencyAmount);

                mailItem.PayOrTakeCash();

                session.EnqueueMessageEncrypted(new ServerMailResult
                {
                    Action = 3,
                    MailId = mailItem.Id,
                    Result = result
                });
            }
        }

        [MessageHandler(GameMessageOpcode.ClientMailReturn)]
        public static void HandleMailReturn(WorldSession session, ClientMailReturn clientMailReturn)
        {
            session.EnqueueMessageEncrypted(new ServerMailResult
            {
                Action = 1,
                MailId = clientMailReturn.MailId,
                Result = 0
            });
            session.EnqueueMessageEncrypted(new ServerMailUnavailable
            {
                MailId = clientMailReturn.MailId
            });
        }

        [MessageHandler(GameMessageOpcode.ClientMailTakeAttachment)]
        public static void HandleMailTakeAttackment(WorldSession session, ClientMailTakeAttachment clientMailTakeAttachment)
        {
            log.Info($"{clientMailTakeAttachment.MailId}, {clientMailTakeAttachment.Index}, {clientMailTakeAttachment.UnitId}");
            session.EnqueueMessageEncrypted(new ServerMailTakeAttachment
            {
                MailId = clientMailTakeAttachment.MailId,
                Result = 0,
                Index = clientMailTakeAttachment.Index
            });
        }

        [MessageHandler(GameMessageOpcode.ClientMailTakeCash)]
        public static void HandleMailTakeCash(WorldSession session, ClientMailTakeCash clientMailTakeCash)
        {
            log.Info($"{clientMailTakeCash.MailId}, {clientMailTakeCash.UnitId}");
            session.EnqueueMessageEncrypted(new ServerMailResult
            {
                Action = 2,
                MailId = clientMailTakeCash.MailId,
                Result = 0
            });
        }

        [MessageHandler(GameMessageOpcode.ClientMailSend)]
        public static void HandleMailSend(WorldSession session, ClientMailSend clientMailSend)
        {
            log.Info($"{clientMailSend.Name}, {clientMailSend.Realm}, {clientMailSend.Subject}, {clientMailSend.Message}, {clientMailSend.CreditsSent}, {clientMailSend.CreditsRequsted}, {clientMailSend.DeliveryTime}, {clientMailSend.UnitId}");

            MailManager.SendMail(session, clientMailSend);

            session.EnqueueMessageEncrypted(new ServerMailResult
            {
                Action = 1,
                MailId = 0,
                Result = 0
            });
        }
    }
}

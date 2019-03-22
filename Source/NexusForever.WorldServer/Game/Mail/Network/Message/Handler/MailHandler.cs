using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
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
            log.Info($"{clientMailDelete.Unknown0}, {clientMailDelete.MailId}");
            session.EnqueueMessageEncrypted(new ServerMailResult
            {
                Action = 5,
                MailId = clientMailDelete.MailId,
                Result = 0
            });
            session.EnqueueMessageEncrypted(new ServerMailUnavailable
            {
                MailId = clientMailDelete.MailId
            });
        }

        [MessageHandler(GameMessageOpcode.ClientMailOpen)]
        public static void HandleMailOpen(WorldSession session, ClientMailOpen clientMailOpen)
        {
           log.Info($"{clientMailOpen.Unknown0}, {clientMailOpen.MailId}");
        }

        [MessageHandler(GameMessageOpcode.ClientMailPayCOD)]
        public static void HandleMailPayCOD(WorldSession session, ClientMailPayCOD clientMailPayCOD)
        {
            log.Info($"{clientMailPayCOD.MailId}, {clientMailPayCOD.UnitId}");
            session.EnqueueMessageEncrypted(new ServerMailResult
            {
                Action = 3,
                MailId = clientMailPayCOD.MailId,
                Result = 0
            });
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

            session.EnqueueMessageEncrypted(new ServerMailResult
            {
                Action = 1,
                MailId = 0,
                Result = 0
            });
        }
    }
}

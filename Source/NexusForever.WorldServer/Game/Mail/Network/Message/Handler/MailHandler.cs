using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;

namespace NexusForever.WorldServer.Game.Mail.Network.Message.Handler
{
    class MailHandler
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [MessageHandler(GameMessageOpcode.ClientMailOpen)]
        public static void HandleMailOpen(WorldSession session, ClientMailOpen clientMailOpen)
        {
           log.Info($"{clientMailOpen.Unknown0}, {clientMailOpen.MailId}");
        }

        [MessageHandler(GameMessageOpcode.ClientMailSend)]
        public static void HandleMailSend(WorldSession session, ClientMailSend clientMailSend)
        {
           log.Info($"{clientMailSend.Name}, {clientMailSend.Realm}, {clientMailSend.Subject}, {clientMailSend.Message}, {clientMailSend.Unknown4}, {clientMailSend.Unknown5}, {clientMailSend.Unknown6}, {clientMailSend.Unknown7}, {clientMailSend.Unknown8}");
        }

        [MessageHandler(GameMessageOpcode.ClientMailTakeCash)]
        public static void HandleMailTakeCash(WorldSession session, ClientMailTakeCash clientMailTakeCash)
        {
           log.Info($"{clientMailTakeCash.MailId}, {clientMailTakeCash.UnitId}");
           session.EnqueueMessageEncrypted(new ServerMailResult
           {
               Unknown0 = 2,
               Unknown1 = clientMailTakeCash.MailId,
               Unknown2 = 0
           });
        }
    }
}

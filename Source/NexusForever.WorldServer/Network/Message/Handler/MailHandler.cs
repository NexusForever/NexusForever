using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Mail;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class MailHandler
    {
        /// <summary>
        /// Handles deletion of a <see cref="MailItem"/>
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientMailDelete)]
        public static void HandleMailDelete(WorldSession session, ClientMailDelete mailDelete)
        {
            foreach (ulong mailId in mailDelete.MailList)
                session.Player.MailManager.MailDelete(mailId);
        }

        /// <summary>
        /// Handles the client opening a new <see cref="MailItem"/>
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientMailOpen)]
        public static void HandleMailOpen(WorldSession session, ClientMailOpen mailOpen)
        {
            foreach (ulong mailId in mailOpen.MailList)
                session.Player.MailManager.MailMarkAsRead(mailId);
        }

        /// <summary>
        /// Handles the client request to pay the credits requested for a <see cref="MailItem"/>
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientMailPayCod)]
        public static void HandleMailPayCod(WorldSession session, ClientMailPayCod mailPayCod)
        {
            session.Player.MailManager.MailPayCod(mailPayCod.MailId);
        }

        /// <summary>
        /// Handles returning a <see cref="MailItem"/> to its original sender
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientMailReturn)]
        public static void HandleMailReturn(WorldSession session, ClientMailReturn mailReturn)
        {
            session.Player.MailManager.ReturnMail(mailReturn.MailId);
        }

        // TODO: Implement ClientMailTakeAllFromSelection

        /// <summary>
        /// Handles a client request to retrieve a <see cref="MailAttachment"/> from a <see cref="MailItem"/>
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientMailTakeAttachment)]
        public static void HandleMailTakeAttachment(WorldSession session, ClientMailTakeAttachment mailTakeAttachment)
        {
            session.Player.MailManager.MailTakeAttachment(mailTakeAttachment.MailId, mailTakeAttachment.Index, mailTakeAttachment.UnitId);
        }

        /// <summary>
        /// Handles a client request to retrieve the currency sent with a <see cref="MailItem"/>
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientMailTakeCash)]
        public static void HandleMailTakeCash(WorldSession session, ClientMailTakeCash mailTakeCash)
        {
            session.Player.MailManager.MailTakeCash(mailTakeCash.MailId, mailTakeCash.UnitId);
        }

        /// <summary>
        /// Handles a client request to send a mail to another player
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientMailSend)]
        public static void HandleMailSend(WorldSession session, ClientMailSend mailSend)
        {
            session.Player.MailManager.SendMail(mailSend);
        }
    }
}

using NexusForever.Game.Abstract.Mail;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class MailHandler
    {
        /// <summary>
        /// Handles deletion of a <see cref="IMailItem"/>
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientMailDelete)]
        public static void HandleMailDelete(IWorldSession session, ClientMailDelete mailDelete)
        {
            foreach (ulong mailId in mailDelete.MailList)
                session.Player.MailManager.MailDelete(mailId);
        }

        /// <summary>
        /// Handles the client opening a new <see cref="IMailItem"/>
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientMailOpen)]
        public static void HandleMailOpen(IWorldSession session, ClientMailOpen mailOpen)
        {
            foreach (ulong mailId in mailOpen.MailList)
                session.Player.MailManager.MailMarkAsRead(mailId);
        }

        /// <summary>
        /// Handles the client request to pay the credits requested for a <see cref="IMailItem"/>
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientMailPayCod)]
        public static void HandleMailPayCod(IWorldSession session, ClientMailPayCod mailPayCod)
        {
            session.Player.MailManager.MailPayCod(mailPayCod.MailId);
        }

        /// <summary>
        /// Handles returning a <see cref="IMailItem"/> to its original sender
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientMailReturn)]
        public static void HandleMailReturn(IWorldSession session, ClientMailReturn mailReturn)
        {
            session.Player.MailManager.ReturnMail(mailReturn.MailId);
        }

        // TODO: Implement ClientMailTakeAllFromSelection

        /// <summary>
        /// Handles a client request to retrieve a <see cref="IMailAttachment"/> from a <see cref="IMailItem"/>
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientMailTakeAttachment)]
        public static void HandleMailTakeAttachment(IWorldSession session, ClientMailTakeAttachment mailTakeAttachment)
        {
            session.Player.MailManager.MailTakeAttachment(mailTakeAttachment.MailId, mailTakeAttachment.Index, mailTakeAttachment.UnitId);
        }

        /// <summary>
        /// Handles a client request to retrieve the currency sent with a <see cref="IMailItem"/>
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientMailTakeCash)]
        public static void HandleMailTakeCash(IWorldSession session, ClientMailTakeCash mailTakeCash)
        {
            session.Player.MailManager.MailTakeCash(mailTakeCash.MailId, mailTakeCash.UnitId);
        }

        /// <summary>
        /// Handles a client request to send a mail to another player
        /// </summary>
        [MessageHandler(GameMessageOpcode.ClientMailSend)]
        public static void HandleMailSend(IWorldSession session, ClientMailSend mailSend)
        {
            session.Player.MailManager.SendMail(mailSend);
        }
    }
}

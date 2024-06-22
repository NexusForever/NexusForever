using NexusForever.Game.Abstract.Mail;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Mail
{
    public class ClientMailTakeCashHandler : IMessageHandler<IWorldSession, ClientMailTakeCash>
    {
        /// <summary>
        /// Handles a client request to retrieve the currency sent with a <see cref="IMailItem"/>
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientMailTakeCash mailTakeCash)
        {
            session.Player.MailManager.MailTakeCash(mailTakeCash.MailId, mailTakeCash.UnitId);
        }
    }
}

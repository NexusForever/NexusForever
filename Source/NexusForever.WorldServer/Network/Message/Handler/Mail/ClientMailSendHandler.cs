using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Mail
{
    public class ClientMailSendHandler : IMessageHandler<IWorldSession, ClientMailSend>
    {
        /// <summary>
        /// Handles a client request to send a mail to another player
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientMailSend mailSend)
        {
            session.Player.MailManager.SendMail(mailSend);
        }
    }
}

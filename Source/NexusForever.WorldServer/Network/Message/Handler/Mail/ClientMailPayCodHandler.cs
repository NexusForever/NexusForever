using NexusForever.Game.Abstract.Mail;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Mail
{
    public class ClientMailPayCodHandler : IMessageHandler<IWorldSession, ClientMailPayCod>
    {
        /// <summary>
        /// Handles the client request to pay the credits requested for a <see cref="IMailItem"/>
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientMailPayCod mailPayCod)
        {
            session.Player.MailManager.MailPayCod(mailPayCod.MailId);
        }
    }
}

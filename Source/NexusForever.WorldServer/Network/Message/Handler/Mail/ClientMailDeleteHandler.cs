using NexusForever.Game.Abstract.Mail;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Mail
{
    public class ClientMailDeleteHandler : IMessageHandler<IWorldSession, ClientMailDelete>
    {
        /// <summary>
        /// Handles deletion of a <see cref="IMailItem"/>
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientMailDelete mailDelete)
        {
            foreach (ulong mailId in mailDelete.MailList)
                session.Player.MailManager.MailDelete(mailId);
        }
    }
}

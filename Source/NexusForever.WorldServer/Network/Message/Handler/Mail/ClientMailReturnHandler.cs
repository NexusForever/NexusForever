using NexusForever.Game.Abstract.Mail;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Mail
{
    public class ClientMailReturnHandler : IMessageHandler<IWorldSession, ClientMailReturn>
    {
        /// <summary>
        /// Handles returning a <see cref="IMailItem"/> to its original sender
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientMailReturn mailReturn)
        {
            session.Player.MailManager.ReturnMail(mailReturn.MailId);
        }
    }
}

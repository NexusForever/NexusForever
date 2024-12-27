using NexusForever.Game.Abstract.Mail;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Mail
{
    public class ClientMailOpenHandler : IMessageHandler<IWorldSession, ClientMailOpen>
    {
        /// <summary>
        /// Handles the client opening a new <see cref="IMailItem"/>
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientMailOpen mailOpen)
        {
            foreach (ulong mailId in mailOpen.MailList)
                session.Player.MailManager.MailMarkAsRead(mailId);
        }
    }
}

using NexusForever.Game.Abstract.Mail;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Mail
{
    public class ClientMailTakeAttachmentHandler : IMessageHandler<IWorldSession, ClientMailTakeAttachment>
    {
        /// <summary>
        /// Handles a client request to retrieve a <see cref="IMailAttachment"/> from a <see cref="IMailItem"/>
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientMailTakeAttachment mailTakeAttachment)
        {
            session.Player.MailManager.MailTakeAttachment(mailTakeAttachment.MailId, mailTakeAttachment.Index, mailTakeAttachment.UnitId);
        }
    }
}

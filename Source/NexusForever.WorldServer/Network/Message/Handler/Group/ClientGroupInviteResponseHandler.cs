using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupInviteResponseHandler : IMessageHandler<IWorldSession, ClientGroupInviteResponse>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientGroupInviteResponseHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupInviteResponse groupInviteResponse)
        {
            messagePublisher.PublishAsync(new GroupPlayerInviteRespondedMessage
            {
                Identity = session.Player.Identity.ToInternalIdentity(),
                GroupId  = groupInviteResponse.GroupId,
                Response = groupInviteResponse.Response
            });
        }
    }
}

using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Friendship;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Friendship
{
    public class ClientFriendshipInviteResponseHandler : IMessageHandler<IWorldSession, ClientFriendshipInviteResponse>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientFriendshipInviteResponseHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientFriendshipInviteResponse message)
        {
            messagePublisher.PublishAsync(
                new FriendshipInviteResponseMessage
                {
                    Invitee  = session.Player.Identity.ToInternalIdentity(),
                    InviteId = message.InviteId,
                    Response = message.Response
                }).FireAndForgetAsync();
        }
    }
}

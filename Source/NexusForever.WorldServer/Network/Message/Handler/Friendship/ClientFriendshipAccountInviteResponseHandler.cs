using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Friendship;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Friendship
{
    public class ClientFriendshipAccountInviteResponseHandler : IMessageHandler<IWorldSession, ClientFriendshipAccountInviteResponse>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientFriendshipAccountInviteResponseHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientFriendshipAccountInviteResponse message)
        {
            messagePublisher.PublishAsync(new FriendshipAccountInviteResponseMessage
            {
                Invitee   = session.Player.Identity.ToInternalIdentity(),
                AccountId = session.Account.Id,
                InviteId  = message.AccountFriendInviteId,
                Response  = message.Response
            }).FireAndForgetAsync();
        }
    }
}

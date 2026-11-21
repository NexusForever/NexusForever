using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Friendship;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Friendship
{
    public class ClientFriendshipAccountInviteMarkedSeenHandler : IMessageHandler<IWorldSession, ClientFriendshipAccountInviteMarkSeen>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientFriendshipAccountInviteMarkedSeenHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientFriendshipAccountInviteMarkSeen message)
        {
            messagePublisher.PublishAsync(new FriendshipAccountInviteMarkSeenMessage
            {
                AccountId             = session.Account.Id,
                AccountFriendInviteId = message.AccountFriendInviteId
            }).FireAndForgetAsync();
        }
    }
}

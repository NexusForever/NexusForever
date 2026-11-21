using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Friendship;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Friendship
{
    public class ClientFriendshipAccountRemoveHandler : IMessageHandler<IWorldSession, ClientFriendshipAccountRemove>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientFriendshipAccountRemoveHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientFriendshipAccountRemove message)
        {
            messagePublisher.PublishAsync(new FriendshipAccountRemoveMessage
            {
                AccountId       = session.Account.Id,
                AccountFriendId = message.AccountFriendId
            }).FireAndForgetAsync();
        }
    }
}

using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Friendship;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Friendship
{
    public class ClientFriendshipAccountPersonalPresenceChangeHandler : IMessageHandler<IWorldSession, ClientFriendshipAccountPersonalPresenceChange>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientFriendshipAccountPersonalPresenceChangeHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientFriendshipAccountPersonalPresenceChange message)
        {
            messagePublisher.PublishAsync(new FriendshipAccountPresenceUpdateMessage
            {
                AccountId = session.Account.Id,
                Presence  = message.Presence,
            }).FireAndForgetAsync();
        }
    }
}

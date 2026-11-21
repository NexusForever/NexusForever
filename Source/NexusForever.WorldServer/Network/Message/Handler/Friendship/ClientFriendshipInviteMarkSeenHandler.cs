using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Friendship;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Friendship
{
    public class ClientFriendshipInviteMarkSeenHandler : IMessageHandler<IWorldSession, ClientFriendshipInviteMarkSeen>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientFriendshipInviteMarkSeenHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientFriendshipInviteMarkSeen message)
        {
            messagePublisher.PublishAsync(new FriendshipInviteMarkSeenMessage
            {
                Identity        = session.Player.Identity.ToInternalIdentity(),
                FriendInviteIds = message.FriendInviteIds
            }).FireAndForgetAsync();
        }
    }
}

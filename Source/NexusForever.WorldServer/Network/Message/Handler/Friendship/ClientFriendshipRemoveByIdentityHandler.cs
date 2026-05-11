using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Friendship;
using NexusForever.Shared;
using NexusForever.WorldServer.Network.Internal;

namespace NexusForever.WorldServer.Network.Message.Handler.Friendship
{
    public class ClientFriendshipRemoveByIdentityHandler : IMessageHandler<IWorldSession, ClientFriendshipRemoveByIdentity>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientFriendshipRemoveByIdentityHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientFriendshipRemoveByIdentity message)
        {
            messagePublisher.PublishAsync(new FriendshipRemoveIdentityMessage
            {
                Inviter = session.Player.Identity.ToInternalIdentity(),
                Invitee = message.PlayerIdentity.ToInternalIdentity(),
                Type    = message.Type
            }).FireAndForgetAsync();
        }
    }
}

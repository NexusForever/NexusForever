using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Friendship;
using NexusForever.Shared;
using NexusForever.WorldServer.Network.Internal;

namespace NexusForever.WorldServer.Network.Message.Handler.Friendship
{
    public class ClientFriendshipAddByIdentityHandler : IMessageHandler<IWorldSession, ClientFriendshipAddByIdentity>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientFriendshipAddByIdentityHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientFriendshipAddByIdentity message)
        {
            messagePublisher.PublishAsync(new FriendshipNameInviteRequestMessage
            {
                Inviter = session.Player.Identity.ToInternalIdentity(),
                Invitee = message.Target.ToInternalIdentity(),
                Type    = message.Type,
                Note    = message.Note != string.Empty ? message.Note : null
            }).FireAndForgetAsync();
        }
    }
}

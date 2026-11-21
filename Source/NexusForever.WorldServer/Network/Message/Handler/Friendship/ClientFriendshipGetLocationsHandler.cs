using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Friendship;
using NexusForever.Shared;
using NexusForever.WorldServer.Network.Internal;

namespace NexusForever.WorldServer.Network.Message.Handler.Friendship
{
    public class ClientFriendshipGetLocationsHandler : IMessageHandler<IWorldSession, ClientFriendshipGetLocations>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientFriendshipGetLocationsHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientFriendshipGetLocations message)
        {
            messagePublisher.PublishAsync(new FriendshipLocationRequestMessage
            {
                Identity       = session.Player.Identity.ToInternalIdentity(),
                Friends        = message.FriendIdentities.ConvertAll(i => i.ToInternalIdentity()),
                AccountFriends = message.AccountFriendIds
            }).FireAndForgetAsync();
        }
    }
}

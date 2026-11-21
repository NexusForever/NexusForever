using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Friendship;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Friendship
{
    public class ClientFriendshipAccountSetPrivateDataHandler : IMessageHandler<IWorldSession, ClientFriendshipAccountSetPrivateData>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientFriendshipAccountSetPrivateDataHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientFriendshipAccountSetPrivateData message)
        {
            messagePublisher.PublishAsync(new FriendshipAccountNoteUpdateMessage
            {
                Identity        = session.Player.Identity.ToInternalIdentity(),
                AccountId       = session.Account.Id,
                FriendAccountId = message.AccountFriendId,
                Note            = message.PrivateDataText
            }).FireAndForgetAsync();
        }
    }
}

using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Friendship;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Friendship
{
    public class ClientFriendshipAccountSetPublicNoteHandler : IMessageHandler<IWorldSession, ClientFriendshipAccountSetPublicNote>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientFriendshipAccountSetPublicNoteHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientFriendshipAccountSetPublicNote message)
        {
            messagePublisher.PublishAsync(new FriendshipAccountStatusUpdateMessage
            {
                Identity  = session.Player.Identity.ToInternalIdentity(),
                AccountId = session.Account.Id,
                Status    = message.PublicNote != string.Empty ? message.PublicNote : null
            }).FireAndForgetAsync();
        }
    }
}

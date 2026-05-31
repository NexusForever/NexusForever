using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Friendship;
using NexusForever.Shared;
using NexusForever.WorldServer.Network.Internal;

namespace NexusForever.WorldServer.Network.Message.Handler.Friendship
{
    public class ClientFriendshipSetNoteByIdentityHandler : IMessageHandler<IWorldSession, ClientFriendshipSetNoteByIdentity>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientFriendshipSetNoteByIdentityHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientFriendshipSetNoteByIdentity message)
        {
            messagePublisher.PublishAsync(new FriendshipNoteUpdateByIdentityMessage
            {
                Source = session.Player.Identity.ToInternalIdentity(),
                Target = message.PlayerIdentity.ToInternalIdentity(),
                Note   = message.Note
            }).FireAndForgetAsync();
        }
    }
}

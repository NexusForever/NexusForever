using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Friendship;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Friendship
{
    public class ClientFriendshipAccountAddByEmailHandler : IMessageHandler<IWorldSession, ClientFriendshipAccountAddByEmail>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientFriendshipAccountAddByEmailHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientFriendshipAccountAddByEmail message)
        {
            messagePublisher.PublishAsync(new FriendshipAccountEmailInviteRequestMessage
            {
                Inviter          = session.Player.Identity.ToInternalIdentity(),
                InviterAccountId = session.Player.Account.Id,
                Email            = message.Email,
                Note             = message.Note
            }).FireAndForgetAsync();
        }
    }
}

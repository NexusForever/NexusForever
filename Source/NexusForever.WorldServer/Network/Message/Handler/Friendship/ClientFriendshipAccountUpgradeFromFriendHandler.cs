using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Friendship;
using NexusForever.Shared;
using NexusForever.WorldServer.Network.Internal;

namespace NexusForever.WorldServer.Network.Message.Handler.Friendship
{
    public class ClientFriendshipAccountUpgradeFromFriendHandler : IMessageHandler<IWorldSession, ClientFriendshipAccountUpgradeFromFriend>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientFriendshipAccountUpgradeFromFriendHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientFriendshipAccountUpgradeFromFriend message)
        {
            messagePublisher.PublishAsync(new FriendshipAccountFriendInviteRequestMessage
            {
                Inviter          = session.Player.Identity.ToInternalIdentity(),
                InviterAccountId = session.Account.Id,
                Invitee          = message.ExisitingFriend.ToInternalIdentity(),
                Note             = message.Note != string.Empty ? message.Note : null
            }).FireAndForgetAsync();
        }
    }
}

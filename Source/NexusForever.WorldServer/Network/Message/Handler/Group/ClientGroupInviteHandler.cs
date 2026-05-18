using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupInviteHandler : IMessageHandler<IWorldSession, ClientGroupInvite>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientGroupInviteHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupInvite groupInvite)
        {
            messagePublisher.PublishAsync(new GroupPlayerInviteMessage
            {
                Inviter = session.Player.Identity.ToInternalIdentity(),
                Invitee = new NexusForever.Network.Internal.Message.Shared.IdentityName
                {
                    Name      = groupInvite.Name,
                    RealmName = groupInvite.RealmName
                }
            }).FireAndForgetAsync();
        }
    }
}

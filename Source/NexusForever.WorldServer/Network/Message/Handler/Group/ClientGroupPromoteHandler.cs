using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;
using NexusForever.WorldServer.Network.Internal;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupPromoteHandler : IMessageHandler<IWorldSession, ClientGroupPromote>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientGroupPromoteHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupPromote groupPromote)
        {
            messagePublisher.PublishAsync(new GroupMemberPromoteMessage
            {
                GroupId  = groupPromote.GroupId,
                Promoter = session.Player.Identity.ToInternalIdentity(),
                Promotee = groupPromote.TargetedPlayer.ToInternalIdentity()

            }).FireAndForgetAsync();
        }
    }
}

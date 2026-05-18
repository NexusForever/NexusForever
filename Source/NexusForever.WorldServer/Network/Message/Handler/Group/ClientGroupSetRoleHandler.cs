using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;
using NexusForever.WorldServer.Network.Internal;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupSetRoleHandler : IMessageHandler<IWorldSession, ClientGroupSetRole>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientGroupSetRoleHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupSetRole groupSetRole)
        {
            messagePublisher.PublishAsync(new GroupMemberFlagUpdateMessage
            {
                GroupId = groupSetRole.GroupId,
                Source  = session.Player.Identity.ToInternalIdentity(),
                Target  = groupSetRole.TargetedPlayer.ToInternalIdentity(),
                Flags   = groupSetRole.ChangedFlag
            }).FireAndForgetAsync();
        }
    }
}

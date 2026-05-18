using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;
using NexusForever.WorldServer.Network.Internal;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupKickHandler : IMessageHandler<IWorldSession, ClientGroupKick>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientGroupKickHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupKick groupKick)
        {
            messagePublisher.PublishAsync(new GroupMemberKickMessage
            {
                GroupId = groupKick.GroupId,
                Kicker  = session.Player.Identity.ToInternalIdentity(),
                Kicked  = groupKick.TargetedPlayer.ToInternalIdentity(),
            }).FireAndForgetAsync();
        }
    }
}

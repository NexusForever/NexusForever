using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupLeaveHandler : IMessageHandler<IWorldSession, ClientGroupLeave>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientGroupLeaveHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupLeave groupLeave)
        {
            if (groupLeave.ShouldDisband)
            {
                messagePublisher.PublishAsync(new GroupDisbandMessage
                {
                    Identity = session.Player.Identity.ToInternalIdentity(),
                    GroupId  = groupLeave.GroupId
                });
            }
            else
            {
                messagePublisher.PublishAsync(new GroupMemberLeaveMessage
                {
                    Identity = session.Player.Identity.ToInternalIdentity(),
                    GroupId  = groupLeave.GroupId
                });
            }
        }
    }
}

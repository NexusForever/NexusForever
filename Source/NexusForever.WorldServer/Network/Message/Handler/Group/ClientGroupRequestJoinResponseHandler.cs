using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupRequestJoinResponseHandler : IMessageHandler<IWorldSession, ClientGroupRequestJoinResponse>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientGroupRequestJoinResponseHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupRequestJoinResponse groupRequestJoinResponse)
        {
            messagePublisher.PublishAsync(new GroupMemberRequestReponseMessage
            {
                GroupId     = groupRequestJoinResponse.GroupId,
                Identity    = session.Player.Identity.ToInternalIdentity(),
                InviteeName = groupRequestJoinResponse.InviteeName,
                Response    = groupRequestJoinResponse.AcceptedRequest,
            }).FireAndForgetAsync();
        }
    }
}

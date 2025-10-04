using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupRequestJoinHandler : IMessageHandler<IWorldSession, ClientGroupRequestJoin>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientGroupRequestJoinHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupRequestJoin joinRequest)
        {
            messagePublisher.PublishAsync(new GroupMemberRequestMessage
            {
                Requester = session.Player.Identity.ToInternalIdentity(),
                Requestee = new NexusForever.Network.Internal.Message.Shared.IdentityName
                {
                    Name      = joinRequest.Name,
                    RealmName = joinRequest.RealmName
                }
            }).FireAndForgetAsync();
        }
    }
}

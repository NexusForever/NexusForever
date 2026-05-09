using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Info;
using NexusForever.Shared;
using NexusForever.WorldServer.Network.Internal;

namespace NexusForever.WorldServer.Network.Message.Handler.Info
{
    public class ClientPlayerInfoRequestHandler : IMessageHandler<IWorldSession, ClientPlayerInfoRequest>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientPlayerInfoRequestHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        /// <summary>
        /// Handled responses to Player Info Requests.
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientPlayerInfoRequest request)
        {
            messagePublisher.PublishAsync(new PlayerInfoRequestMessage
            {
                Source = session.Player.Identity.ToInternalIdentity(),
                Type   = request.Type,
                Target = request.Identity.ToInternalIdentity()
            }).FireAndForgetAsync();
        }
    }
}

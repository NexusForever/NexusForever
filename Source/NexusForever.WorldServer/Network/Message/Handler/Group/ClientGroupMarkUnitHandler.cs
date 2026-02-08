using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    internal class ClientGroupMarkUnitHandler : IMessageHandler<IWorldSession, ClientGroupMark>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientGroupMarkUnitHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupMark groupMark)
        {
            messagePublisher.PublishAsync(new GroupMarkerMessage
            {
                MarkerIndentity = session.Player.Identity.ToInternalIdentity(),
                GroupMarker     = groupMark.Marker,
                UnitId          = groupMark.UnitId != 0 ? groupMark.UnitId : null,
            }).FireAndForgetAsync();
        }
    }
}

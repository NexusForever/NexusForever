using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity
{
    public class ClientEntityTargetHandler : IMessageHandler<IWorldSession, ClientEntitySelect>
    {
        public void HandleMessage(IWorldSession session, ClientEntitySelect entitySelect)
        {
            session.Player.SetTarget(entitySelect.Guid > 0 ? entitySelect.Guid : null);
        }
    }
}

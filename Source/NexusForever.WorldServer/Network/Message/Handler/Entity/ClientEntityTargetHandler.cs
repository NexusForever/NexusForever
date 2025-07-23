using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Player;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity
{
    public class ClientEntityTargetHandler : IMessageHandler<IWorldSession, ClientEntitySelect>
    {
        public void HandleMessage(IWorldSession session, ClientEntitySelect entitySelect)
        {
            session.Player.SetTarget(entitySelect.UnitId > 0 ? entitySelect.UnitId : null);
        }
    }
}

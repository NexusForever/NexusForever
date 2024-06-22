using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity
{
    public class ClientEntityCommandHandler : IMessageHandler<IWorldSession, ClientEntityCommand>
    {
        public void HandleMessage(IWorldSession session, ClientEntityCommand entityCommand)
        {
            IWorldEntity mover = session.Player;
            if (mover == null)
                return;

            if (session.Player.ControlGuid != null)
                mover = session.Player.GetVisible<IWorldEntity>(session.Player.ControlGuid.Value);

            mover?.MovementManager.HandleClientEntityCommands(entityCommand.Commands, entityCommand.Time);
        }
    }
}

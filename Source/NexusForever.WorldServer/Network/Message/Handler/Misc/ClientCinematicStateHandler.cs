using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Misc
{
    public class ClientCinematicStateHandler : IMessageHandler<IWorldSession, ClientCinematicState>
    {
        public void HandleMessage(IWorldSession session, ClientCinematicState cinematicState)
        {
            session.Player.CinematicManager.HandleClientCinematicState(cinematicState.State);
        }
    }
}

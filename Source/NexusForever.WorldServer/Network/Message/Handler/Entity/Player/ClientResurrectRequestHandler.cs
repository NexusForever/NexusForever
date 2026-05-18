using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity.Player
{
    public class ClientResurrectRequestHandler : IMessageHandler<IWorldSession, ClientResurrectRequest>
    {
        public void HandleMessage(IWorldSession session, ClientResurrectRequest _)
        {
            if (session.Player.TargetGuid == null)
                return;

            session.Player.ResurrectionManager.Resurrect(session.Player.TargetGuid.Value);
        }
    }
}

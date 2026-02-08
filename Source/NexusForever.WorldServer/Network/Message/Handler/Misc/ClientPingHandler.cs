using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Pregame;

namespace NexusForever.WorldServer.Network.Message.Handler.Misc
{
    public class ClientPingHandler : IMessageHandler<IWorldSession, ClientPregameKeepAlive>
    {
        public void HandleMessage(IWorldSession session, ClientPregameKeepAlive ping)
        {
            session.Heartbeat.OnHeartbeat();
        }
    }
}

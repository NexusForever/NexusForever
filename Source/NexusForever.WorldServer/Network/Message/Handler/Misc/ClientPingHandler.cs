using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Misc
{
    public class ClientPingHandler : IMessageHandler<IWorldSession, ClientPing>
    {
        public void HandleMessage(IWorldSession session, ClientPing ping)
        {
            session.Heartbeat.OnHeartbeat();
        }
    }
}

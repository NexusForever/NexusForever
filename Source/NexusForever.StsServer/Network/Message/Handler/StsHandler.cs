using NexusForever.StsServer.Network.Message.Model;

namespace NexusForever.StsServer.Network.Message.Handler
{
    public static class StsHandler
    {
        [MessageHandler("/Sts/Connect", SessionState.None)]
        public static void HandleConnect(StsSession session, ClientConnectMessage connect)
        {
            session.State = SessionState.Connected;
        }

        [MessageHandler("/Sts/Ping", SessionState.None)]
        public static void HandlePing(StsSession session, PingMessage ping)
        {
            session.Heartbeat.OnHeartbeat();
        }
    }
}

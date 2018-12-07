using NexusForever.Shared.Game.Events;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;

namespace NexusForever.WorldServer.Network.Message.Handler
{

    public static class ClientLogoutHandler
    {
        private static Logger Log { get; } = LogManager.GetCurrentClassLogger();

        [MessageHandler(GameMessageOpcode.ClientLogout)]
        public static void HandleLogout(WorldSession session, ClientLogout logout)
        {
            Log.Info("Session logout for character {0}", session.Player.CharacterId);
            session.EnqueueMessageEncrypted(new ServerClientLogout
            {
                ClientRequested = true,
                Reason = LogoutReason.None
            });
            //session.EnqueueEvent(new DisconnectClientEvent(session));
        }

        private class DisconnectClientEvent : IEvent
        {
            private readonly WorldSession session;

            public DisconnectClientEvent(WorldSession session)
            {
                this.session = session;
            }

            public bool CanExecute()
            {
                return true;
            }

            public void Execute()
            {
                if (!session.Disconnected)
                    session.Disconnect();
            }
        }
    }
}

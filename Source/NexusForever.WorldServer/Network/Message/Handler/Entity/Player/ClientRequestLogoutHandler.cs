using NexusForever.Game.Static.RBAC;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity.Player
{
    public class ClientRequestLogoutHandler : IMessageHandler<IWorldSession, ClientLogoutRequest>
    {
        public void HandleMessage(IWorldSession session, ClientLogoutRequest logoutRequest)
        {
            if (logoutRequest.Initiated)
            {
                bool instantLogout = session.Account.RbacManager.HasPermission(Permission.InstantLogout);
                session.Player.LogoutManager.Start(instantLogout ? 0D : 30D);
            }
            else
                session.Player.LogoutManager.Cancel();
        }
    }
}

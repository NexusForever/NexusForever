using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity.Player
{
    public class ClientLogoutConfirmHandler : IMessageHandler<IWorldSession, ClientLogoutConfirm>
    {
        public void HandleMessage(IWorldSession session, ClientLogoutConfirm _)
        {
            session.Player.LogoutManager.Finish();
        }
    }
}

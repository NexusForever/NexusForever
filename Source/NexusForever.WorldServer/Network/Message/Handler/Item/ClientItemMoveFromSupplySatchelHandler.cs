using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Item
{
    public class ClientItemMoveFromSupplySatchelHandler : IMessageHandler<IWorldSession, ClientItemMoveFromSupplySatchel>
    {
        public void HandleMessage(IWorldSession session, ClientItemMoveFromSupplySatchel request)
        {
            session.Player.SupplySatchelManager.MoveToInventory(request.MaterialId, request.Amount);
        }
    }
}

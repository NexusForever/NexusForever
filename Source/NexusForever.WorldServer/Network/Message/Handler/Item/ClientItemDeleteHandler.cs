using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Item
{
    public class ClientItemDeleteHandler : IMessageHandler<IWorldSession, ClientItemDelete>
    {
        public void HandleMessage(IWorldSession session, ClientItemDelete itemDelete)
        {
            session.Player.Inventory.ItemDelete(itemDelete.From);
        }
    }
}

using NexusForever.Game.Abstract.Entity;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.WorldServer.Network.Message.Handler.Item
{
    public class ClientItemMoveHandler : IMessageHandler<IWorldSession, ClientItemMove>
    {
        public void HandleMessage(IWorldSession session, ClientItemMove itemMove)
        {
            IItem item = session.Player.Inventory.GetItem(itemMove.From);
            if (item == null)
                throw new InvalidPacketValueException();

            GenericError? result = session.Player.Inventory.CanMoveItem(item, itemMove.To);
            if (result.HasValue)
            {
                session.EnqueueMessageEncrypted(new ServerItemError
                {
                    ItemGuid  = item.Guid,
                    ErrorCode = result.Value
                });
                return;
            }

            session.Player.Inventory.ItemMove(item, itemMove.To);
        }
    }
}

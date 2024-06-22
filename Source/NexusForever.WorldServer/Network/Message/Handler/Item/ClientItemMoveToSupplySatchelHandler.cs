using NexusForever.Game.Abstract.Entity;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Item
{
    public class ClientItemMoveToSupplySatchelHandler : IMessageHandler<IWorldSession, ClientItemMoveToSupplySatchel>
    {
        public void HandleMessage(IWorldSession session, ClientItemMoveToSupplySatchel moveToSupplySatchel)
        {
            IItem item = session.Player.Inventory.GetItem(moveToSupplySatchel.ItemGuid);
            if (item == null)
                throw new InvalidPacketValueException();

            session.Player.Inventory.ItemMoveToSupplySatchel(item, moveToSupplySatchel.Amount);
        }
    }
}

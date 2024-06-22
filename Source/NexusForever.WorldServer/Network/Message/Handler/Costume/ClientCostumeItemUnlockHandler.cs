using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Costume
{
    public class ClientCostumeItemUnlockHandler : IMessageHandler<IWorldSession, ClientCostumeItemUnlock>
    {
        public void HandleMessage(IWorldSession session, ClientCostumeItemUnlock costumeItemUnlock)
        {
            IItem item = session.Player.Inventory.GetItem(costumeItemUnlock.Location);
            session.Player.Account.CostumeManager.UnlockItem(item);
        }
    }
}

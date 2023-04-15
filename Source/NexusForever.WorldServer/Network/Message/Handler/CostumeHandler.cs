using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class CostumeHandler
    {
        [MessageHandler(GameMessageOpcode.ClientCostumeSave)]
        public static void HandleCostumeSave(IWorldSession session, ClientCostumeSave costumeSave)
        {
            session.Player.CostumeManager.SaveCostume(costumeSave);
        }

        [MessageHandler(GameMessageOpcode.ClientCostumeSet)]
        public static void ClientCostumeSet(IWorldSession session, ClientCostumeSet costumeSet)
        {
            session.Player.CostumeManager.SetCostume(costumeSet.Index);
        }

        [MessageHandler(GameMessageOpcode.ClientCostumeItemUnlock)]
        public static void HandleCostumeItemUnlock(IWorldSession session, ClientCostumeItemUnlock costumeItemUnlock)
        {
            IItem item = session.Player.Inventory.GetItem(costumeItemUnlock.Location);
            session.Player.Account.CostumeManager.UnlockItem(item);
        }

        [MessageHandler(GameMessageOpcode.ClientCostumeItemForget)]
        public static void HandleCostumeItemForget(IWorldSession session, ClientCostumeItemForget costumeItemForget)
        {
            session.Player.Account.CostumeManager.ForgetItem(costumeItemForget.ItemId);
        }
    }
}

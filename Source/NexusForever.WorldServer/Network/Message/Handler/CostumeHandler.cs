using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class CostumeHandler
    {
        [MessageHandler(GameMessageOpcode.ClientCostumeSave)]
        public static void HandleCostumeSave(WorldSession session, ClientCostumeSave costumeSave)
        {
            session.Player.CostumeManager.SaveCostume(costumeSave);
        }

        [MessageHandler(GameMessageOpcode.ClientCostumeSet)]
        public static void ClientCostumeSet(WorldSession session, ClientCostumeSet costumeSet)
        {
            session.Player.CostumeManager.SetCostume(costumeSet.Index);
        }

        [MessageHandler(GameMessageOpcode.ClientCostumeItemUnlock)]
        public static void HandleCostumeItemUnlock(WorldSession session, ClientCostumeItemUnlock costumeItemUnlock)
        {
            session.Player.CostumeManager.UnlockItem(costumeItemUnlock.Location);
        }

        [MessageHandler(GameMessageOpcode.ClientCostumeItemForget)]
        public static void HandleCostumeItemForget(WorldSession session, ClientCostumeItemForget costumeItemForget)
        {
            session.Player.CostumeManager.ForgetItem(costumeItemForget.ItemId);
        }
    }
}

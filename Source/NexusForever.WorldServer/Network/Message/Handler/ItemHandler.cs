using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class ItemHandler
    {
        [MessageHandler(GameMessageOpcode.ClientItemMove)]
        public static void HandleItemMove(WorldSession session, ClientItemMove itemMove)
        {
            session.Player.Inventory.ItemMove(itemMove.From, itemMove.To);
        }

        [MessageHandler(GameMessageOpcode.ClientItemSplit)]
        public static void HandleItemSplit(WorldSession session, ClientItemSplit itemSplit)
        {
            session.Player.Inventory.ItemSplit();
        }

        [MessageHandler(GameMessageOpcode.ClientItemDelete)]
        public static void HandleItemDelete(WorldSession session, ClientItemDelete itemDelete)
        {
            session.Player.Inventory.ItemDelete(itemDelete.From);
        }
    }
}

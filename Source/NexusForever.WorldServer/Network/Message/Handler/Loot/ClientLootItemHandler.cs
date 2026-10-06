using NexusForever.Game.Abstract.Loot;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Loot;

namespace NexusForever.WorldServer.Network.Message.Handler.Loot
{
    public class ClientLootItemHandler : IMessageHandler<IWorldSession, ClientLootItem>
    {
        public void HandleMessage(IWorldSession session, ClientLootItem lootItem)
        {
            ILootInstanceItem lootInstanceItem =
                session.Player.Map.LootManager.GetLootItemInstance(lootItem.OwnerUnitId, lootItem.LootUnitId);
            if (lootInstanceItem == null)
                return;

            if (lootItem.Request)
            {
                // TODO
            }
            else
                lootInstanceItem.TakeLoot(session.Player);
        }
    }
}

using NexusForever.Game.Abstract.Loot;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Loot;

namespace NexusForever.WorldServer.Network.Message.Handler.Loot
{
    public class ClientLootRollActionHandler : IMessageHandler<IWorldSession, ClientLootRollAction>
    {
        public void HandleMessage(IWorldSession session, ClientLootRollAction lootRollAction)
        {
            ILootInstanceItem lootInstanceItem =
                session.Player.Map.LootManager.GetLootItemInstance(lootRollAction.OwnerUnitId, lootRollAction.LootUnitId);
            if (lootInstanceItem == null)
                return;

            lootInstanceItem.SetRollAction(session.Player.Identity, lootRollAction.Action);
        }
    }
}

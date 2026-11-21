using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Loot;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Loot;

namespace NexusForever.WorldServer.Network.Message.Handler.Loot
{
    public class ClientLootAssignMasterHandler : IMessageHandler<IWorldSession, ClientLootAssignMaster>
    {
        public void HandleMessage(IWorldSession session, ClientLootAssignMaster lootAssignMaster)
        {
            ILootInstanceItem lootInstanceItem =
                session.Player.Map.LootManager.GetLootItemInstance(lootAssignMaster.OwnerUnitId, lootAssignMaster.LootUnitId);
            if (lootInstanceItem == null)
                return;

            if (!lootInstanceItem.IsMasterLooter(session.Player.Identity))
                return;

            IPlayer assignee = session.Player.Map.PlayerManager.GetPlayer(lootAssignMaster.Assignee.ToGameIdentity());
            if (assignee == null)
                return;

            lootInstanceItem.AssignLoot(assignee);
        }
    }
}

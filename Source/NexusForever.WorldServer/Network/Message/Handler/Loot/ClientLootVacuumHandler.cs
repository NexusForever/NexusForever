using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Loot;

namespace NexusForever.WorldServer.Network.Message.Handler.Loot
{
    public class ClientLootVacuumHandler : IMessageHandler<IWorldSession, ClientLootVacuum>
    {
        public void HandleMessage(IWorldSession session, ClientLootVacuum lootVacuum)
        {
            session.Player.Map.LootManager.VacuumLoot(session.Player);
        }
    }
}

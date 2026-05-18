using NexusForever.Game.Loot;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Loot;

namespace NexusForever.WorldServer.Network.Message.Handler.Loot
{
    public class ClientLootItemHandler : IMessageHandler<IWorldSession, ClientLootItem>
    {
        public void HandleMessage(IWorldSession session, ClientLootItem packet)
        {
            GlobalLootManager.Instance.GiveLoot(session.Player, (int)packet.LootUnitId);
        }
    }
}

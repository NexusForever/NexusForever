using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Item;

namespace NexusForever.WorldServer.Network.Message.Handler.Item
{
    public class ClientItemSplitHandler : IMessageHandler<IWorldSession, ClientItemSplit>
    {
        public void HandleMessage(IWorldSession session, ClientItemSplit itemSplit)
        {
            session.Player.Inventory.ItemSplit(itemSplit.ItemGuid, itemSplit.Location, itemSplit.Count);
        }
    }
}

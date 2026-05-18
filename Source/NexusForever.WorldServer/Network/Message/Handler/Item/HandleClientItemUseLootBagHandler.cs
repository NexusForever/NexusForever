using System;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Loot;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Item;

namespace NexusForever.WorldServer.Network.Message.Handler.Item
{
    public class HandleClientItemUseLootBagHandler : IMessageHandler<IWorldSession, ClientItemUseLootBag>
    {
        public void HandleMessage(IWorldSession session, ClientItemUseLootBag useLootBag)
        {
            IItem item = session.Player.Inventory.GetItem(useLootBag.ItemLocation);
            if (item == null)
                throw new ArgumentException($"Item missing at Inventory Location {useLootBag.ItemLocation.Location} and Index {useLootBag.ItemLocation.BagIndex}.");

            if (useLootBag.Guid != item.Guid)
                throw new InvalidOperationException($"Guid {useLootBag.Guid} received does not match the Item found at Inventory Location {useLootBag.ItemLocation.Location} and Index {useLootBag.ItemLocation.BagIndex}.");

            if (item.Info.Entry.Item2CategoryId != 138)
                throw new NotImplementedException();

            if (session.Player.Inventory.ItemUse(item))
                GlobalLootManager.Instance.DropLoot(session.Player, item);
        }
    }
}

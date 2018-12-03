using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Database.World.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Linq;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class VendorHandler
    {

        [MessageHandler(GameMessageOpcode.ClientVendor)]
        public static void HandleClientVendor(WorldSession session, ClientVendor vendor)
        {
            var vendorEntity = session.Player.Map.GetEntity<NonPlayer>(vendor.Guid);
            if (vendorEntity == null)
            {
                return;
            }

            if (vendorEntity.VendorInfo == null)
            {
                return;
            }

            session.Player.SelectedVendorInfo = vendorEntity.VendorInfo;
            var serverVendor = new ServerVendorItemsUpdated
            {
                Guid = vendor.Guid,
                SellPriceMultiplier = vendorEntity.VendorInfo.SellPriceMultiplier,
                BuyPriceMultiplier = vendorEntity.VendorInfo.BuyPriceMultiplier,
                Unknown2 = true,
                Unknown3 = true,
                Unknown4 = false
            };

            foreach (EntityVendorCategory category in vendorEntity.VendorInfo.Categories)
            {
                serverVendor.Categories.Add(new ServerVendorItemsUpdated.Category
                {
                    Index = category.Index,
                    LocalisedTextId = category.LocalisedTextId
                });
            }
            foreach (EntityVendorItem item in vendorEntity.VendorInfo.Items)
            {
                serverVendor.Items.Add(new ServerVendorItemsUpdated.Item
                {
                    Index = item.Index,
                    ItemId = item.ItemId,
                    CategoryIndex = item.CategoryIndex,
                    Unknown6 = 0,
                    UnknownB = new[]
                    {
                        new ServerVendorItemsUpdated.Item.UnknownItemStructure(),
                        new ServerVendorItemsUpdated.Item.UnknownItemStructure()
                    }
                });
            }
            session.EnqueueMessageEncrypted(serverVendor);
        }
    }
}

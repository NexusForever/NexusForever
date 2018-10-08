using NexusForever.Shared.Database.World.Model;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Command;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class EntityHandler
    {
        [MessageHandler(GameMessageOpcode.ClientEntityCommand)]
        public static void HandleClientEntityCommand(WorldSession session, ClientEntityCommand entityCommand)
        {
            foreach ((EntityCommand id, IEntityCommand command) in entityCommand.Commands)
            {
                switch (command)
                {
                    case SetPositionCommand setPosition:
                        session.Player.Map.EnqueueRelocate(session.Player, setPosition.Position.Vector);
                        break;
                    case SetRotationCommand setRotation:
                        session.Player.Rotation = setRotation.Position.Vector;
                        break;
                }
            }

            session.Player.EnqueueToVisible(new ServerEntityCommand
            {
                Guid     = session.Player.Guid,
                Time     = entityCommand.Time,
                Unknown2 = true,
                Commands = entityCommand.Commands
            });
        }

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

            var serverVendor = new ServerVendor
            {
                Guid                = vendor.Guid,
                SellPriceMultiplier = vendorEntity.VendorInfo.SellPriceMultiplier,
                BuyPriceMultiplier  = vendorEntity.VendorInfo.BuyPriceMultiplier,
                Unknown2            = true,
                Unknown3            = true,
                Unknown4            = false
            };

            foreach (EntityVendorCategory category in vendorEntity.VendorInfo.Categories)
            {
                serverVendor.Categories.Add(new ServerVendor.Category
                {
                    Index           = category.Index,
                    LocalisedTextId = category.LocalisedTextId
                });
            }
            foreach (EntityVendorItem item in vendorEntity.VendorInfo.Items)
            {
                serverVendor.Items.Add(new ServerVendor.Item
                {
                    Index         = item.Index,
                    ItemId        = item.ItemId,
                    CategoryIndex = item.CategoryIndex,
                    Unknown6      = 0,
                    UnknownB      = new[]
                    {
                        new ServerVendor.Item.UnknownItemStructure(),
                        new ServerVendor.Item.UnknownItemStructure()
                    }
                });
            }
            session.EnqueueMessageEncrypted(serverVendor);
        }
    }
}

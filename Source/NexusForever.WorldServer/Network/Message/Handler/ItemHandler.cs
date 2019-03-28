using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Housing;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Game.Spell;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class ItemHandler
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

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

        [MessageHandler(GameMessageOpcode.ClientItemUse)]
        public static void HandleClientItemUse(WorldSession session, ClientItemUse itemUse)
        {
            Item item = session.Player.Inventory.GetItem(itemUse.Location, itemUse.BagIndex);
            if (item == null)
                throw new InvalidPacketValueException();

            ItemSpecialEntry itemSpecial = GameTableManager.ItemSpecial.GetEntry(item.Entry.ItemSpecialId00);
            if(itemSpecial == null)
                throw new InvalidPacketValueException();

            if (itemSpecial.Spell4IdOnActivate > 0)
                if (session.Player.Inventory.ItemUse(item))
                    session.Player.CastSpell(itemSpecial.Spell4IdOnActivate, new SpellParameters());
        }

        [MessageHandler(GameMessageOpcode.ClientItemUseDye)]
        public static void HandleClientItemUseDye(WorldSession session, ClientItemUseDye itemUseDye)
        {
            Item item = session.Player.Inventory.GetItem(itemUseDye.Location, itemUseDye.BagIndex);
            if (item == null)
                throw new InvalidPacketValueException();
            
            if(item.Entry.GenericUnlockSetId > 0)
            {
                GenericUnlockEntryEntry entry = GameTableManager.GenericUnlockEntry.GetEntry(item.Entry.GenericUnlockSetId);
                if (entry != null)
                    if (!session.GenericUnlockManager.IsDyeUnlocked(entry.UnlockObject))
                        if(session.Player.Inventory.ItemUse(item))
                        {
                            session.GenericUnlockManager.Unlock((ushort)entry.Id);
                            return;
                        }
            }
            
            session.EnqueueMessageEncrypted(new ServerChat
            {
                Guid = session.Player.Guid,
                Channel = Game.Social.ChatChannel.System,
                Text = $"Item not implemented for use."
            });
        }

        [MessageHandler(GameMessageOpcode.ClientItemUseDecor)]
        public static void HandleClientItemUseDecor(WorldSession session, ClientItemUseDecor itemUseDecor)
        {
            Item item = session.Player.Inventory.GetItem(itemUseDecor.ItemGuid);
            if (item == null)
                throw new InvalidPacketValueException();

            if(item.Entry.HousingDecorInfoId > 0)
            {
                HousingDecorInfoEntry entry = GameTableManager.HousingDecorInfo.GetEntry(item.Entry.HousingDecorInfoId);
                if(entry != null)
                {
                    if (!(session.Player.Map is ResidenceMap residenceMap))
                    {
                        session.EnqueueMessageEncrypted(new ServerChat
                        {
                            Guid = session.Player.Guid,
                            Channel = Game.Social.ChatChannel.System,
                            Text = "You need to be on a housing map to use this command!"
                        });
                        return;
                    }

                    if(session.Player.Inventory.ItemUse(item))
                    {
                        residenceMap.DecorCreate(entry, 1);
                        return;
                    }
                }
            }

            session.EnqueueMessageEncrypted(new ServerChat
            {
                Guid = session.Player.Guid,
                Channel = Game.Social.ChatChannel.System,
                Text = $"Item not implemented for use."
            });
        }
    }
}

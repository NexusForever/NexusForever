using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Command;
using NexusForever.WorldServer.Database.Character;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Network.Message.Model;
using Item = NexusForever.WorldServer.Game.Entity.Item;
using NLog;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class SpellHandler
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [MessageHandler(GameMessageOpcode.ClientCastSpell)]
        public static void HandlePlayerCastSpell(WorldSession session, ClientCastSpell spell)
        {
            ushort bagIndex = spell.BagIndex;
            uint guid = spell.Guid;
			
			Item Spell = session.Player.Inventory.GetBag(InventoryLocation.Ability).GetItem(bagIndex);
			
			log.Trace($"cast spell {Spell.SpellEntry.Id}");

            if (Spell.SpellEntry.Id != 0)
            {
                Spell4BaseEntry spell4BaseEntry = GameTableManager.Spell4Base.GetEntry(Spell.SpellEntry.Id);
                if (spell4BaseEntry == null)
                    throw (new InvalidPacketValueException("HandleSpell: Invalid Spell4BaseEntry {Spell.SpellEntry.Id}"));
            }

			// 1: [...] 
			// 2: pew pew
			// 3: :-)
        }
    }
}

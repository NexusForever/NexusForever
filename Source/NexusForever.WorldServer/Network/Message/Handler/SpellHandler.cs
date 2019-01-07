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

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class SpellHandler
    {
        [MessageHandler(GameMessageOpcode.ClientCastSpell)]
        public static void HandlePlayerCastSpell(WorldSession session, ClientCastSpell spell)
        {
            ushort bagIndex = spell.BagIndex;
            uint guid = spell.Guid;
            Random random = new Random();

            Item Spell = session.Player.Inventory.GetBag(InventoryLocation.Ability).GetItem(bagIndex);

            if (Spell.SpellEntry.Id != 0)
            {
                Spell4BaseEntry spell4BaseEntry = GameTableManager.Spell4Base.GetEntry(Spell.SpellEntry.Id);
                if (spell4BaseEntry == null)
                    throw (new InvalidPacketValueException("HandleSpell: Invalid Spell4BaseEntry {Spell.SpellEntry.Id}"));

                Spell4EffectsEntry matchSpell4Effects = Array.Find(GameTableManager.Spell4Effects.Entries, x => x.SpellId == Spell.SpellEntry.Id);
            }

            if (spell.Unknown48 == true) // probably "begin casting"
            {
                Spell4Entry matchSpell4 = Array.Find(GameTableManager.Spell4.Entries, x => x.Spell4BaseIdBaseSpell == Spell.SpellEntry.Id && x.TierIndex == 1);

                // FIXME: this should be a global server increment
                uint CastingId = (uint)random.Next();

                session.Player.EnqueueToVisible(new Server07FF
                {
                    CastingId       = CastingId,
                    CasterId        = session.Player.Guid,
                    Guid2           = session.Player.Guid, // possibly replace with (if set) target
                    Spell4Id        = matchSpell4.Id,
                    RootSpell4Id    = matchSpell4.Id,
                    ParentSpell4Id  = 0,
                    FieldPosition   = new Position(session.Player.Position),
                    UserInitiatedSpellCast = true
                }, true);

                List<Server07F4.UnknownStructure1> unknownStructure1 = new List<Server07F4.UnknownStructure1>
                {
                    new Server07F4.UnknownStructure1
                    {
                     Spell4EffectId = matchSpell4Effects,
                     Unknown0 = 4722,
                     Unknown2 = 4294967295
                    }
                };

                List<Server07F4.UnknownStructure0> unknownStructure0 = new List<Server07F4.UnknownStructure0>
                {
                    new Server07F4.UnknownStructure0
                    {
                        TargetId = session.Player.Guid, // FIXME: insert target
                        Unknown2 = 1,
                        Unknown3 = 1,
                        Unknown4 = 2,
                        unknownStructure1 = unknownStructure1
                    }
                };

                session.Player.EnqueueToVisible(new Server07F4
                {
                    CastingId       = CastingId,
                    Position        = new Position(session.Player.Position),
                    unknownStructure0 = unknownStructure0,
                    Unknown1 = 255
                }, true);
            }
        }
    }
}

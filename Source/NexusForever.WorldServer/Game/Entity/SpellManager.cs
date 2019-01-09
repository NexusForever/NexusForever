using System;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Spell;
using NexusForever.WorldServer.Game.Spell.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Game.Entity
{
    public class SpellManager
    {
        private readonly Player player;
        private readonly Dictionary<uint/*Spell4BaseId*/, UnlockedSpell> spells = new Dictionary<uint, UnlockedSpell>();
        private readonly ActionSet[] actionSets = new ActionSet[ActionSet.MaxActionSets];
        private byte activeActionSet;

        /// <summary>
        /// Create a new <see cref="SpellManager"/> from existing <see cref="Character"/> database model.
        /// </summary>
        public SpellManager(Player owner, Character model)
        {
            player = owner;
            for (byte i = 0; i < ActionSet.MaxActionSets; i++)
                actionSets[i] = new ActionSet(i);
        }

        /// <summary>
        /// Add a new <see cref="UnlockedSpell"/> created from supplied spell base id and tier.
        /// </summary>
        public void AddSpell(uint spell4BaseId, byte tier = 1)
        {
            Spell4BaseEntry entry = GameTableManager.Spell4Base.GetEntry(spell4BaseId);
            if (entry == null)
                throw new ArgumentOutOfRangeException();

            if (spells.ContainsKey(spell4BaseId))
                throw new InvalidOperationException();

            // TODO: validate tier

            Item item = player.Inventory.SpellCreate(entry, 49);

            var unlockedSpell = new UnlockedSpell(entry, tier, item);
            if (!player.IsLoading)
            {
                player.Session.EnqueueMessageEncrypted(new ServerSpellUpdate
                {
                    Spell4BaseId = spell4BaseId,
                    TierIndex    = tier,
                    Activated    = true
                });
            }

            spells.Add(entry.Id, unlockedSpell);
        }

        /// <summary>
        /// Returns <see cref="UnlockedSpell"/> for an existing spell.
        /// </summary>
        public UnlockedSpell GetSpell(uint spell4BaseId)
        {
            return spells.TryGetValue(spell4BaseId, out UnlockedSpell spell) ? spell : null;
        }

        /// <summary>
        /// Add an existing spell to the specified action set.
        /// </summary>
        public void AddSpellToActionSet(byte actionSetIndex, uint spell4BaseId, UILocation location, byte tier = 1)
        {
            if (actionSetIndex >= ActionSet.MaxActionSets)
                throw new ArgumentOutOfRangeException();

            if (!spells.TryGetValue(spell4BaseId, out UnlockedSpell spell))
                throw new ArgumentOutOfRangeException();

            actionSets[actionSetIndex].AddSpell(spell, location, tier);
            spell.Tier = tier;
        }

        public void SendInitialPackets()
        {
            foreach (Item spell in player.Inventory
                .Where(b => b.Location == InventoryLocation.Ability)
                .SelectMany(s => s))
            {
                player.Session.EnqueueMessageEncrypted(new ServerItemAdd
                {
                    InventoryItem = new InventoryItem
                    {
                        Item   = spell.BuildNetworkItem(),
                        Reason = 49
                    }
                });
            }

            var serverSpellList = new ServerSpellList();
            foreach ((uint spell4BaseId, UnlockedSpell spell) in spells)
            {
                serverSpellList.Spells.Add(new ServerSpellList.Spell
                {
                    Spell4BaseId      = spell4BaseId,
                    TierIndexAchieved = spell.Tier
                });
            }

            player.Session.EnqueueMessageEncrypted(serverSpellList);

            player.Session.EnqueueMessageEncrypted(new ServerAbilityPoints
            {
                AbilityPoints      = actionSets[activeActionSet].TierPoints,
                TotalAbilityPoints = ActionSet.MaxTierPoints
            });

            foreach (ActionSet actionSet in actionSets)
            {
                var serverActionSet = new ServerActionSet
                {
                    Index    = actionSet.Index,
                    Unknown3 = 1,
                    Unknown5 = 1
                };

                foreach (ActionSetAction action in actionSet)
                {
                    serverActionSet.Actions.Add(new ServerActionSet.Action
                    {
                        ShortcutType = action.ShortcutType,
                        ObjectId     = action.ObjectId,
                        Location     = new ItemLocation
                        {
                            Location = InventoryLocation.Ability,
                            BagIndex = (uint)action.Location
                        }
                    });
                }

                // initial action set packet requires all slots to be sent
                uint count = (uint)serverActionSet.Actions.Count;
                for (byte i = 0; i < ActionSet.MaxActionCount - count; i++)
                {
                    serverActionSet.Actions.Add(new ServerActionSet.Action
                    {
                        ShortcutType = 0,
                        ObjectId     = 0,
                        Location     = new ItemLocation
                        {
                            Location = (InventoryLocation)300, // no idea why 300, this is what retail did
                            BagIndex = count + i
                        }
                    });
                }

                player.Session.EnqueueMessageEncrypted(serverActionSet);
            }
        }
    }
}

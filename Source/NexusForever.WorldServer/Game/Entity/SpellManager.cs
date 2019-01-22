using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.GameTable.Static;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Spell;
using NexusForever.WorldServer.Game.Spell.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NLog;

namespace NexusForever.WorldServer.Game.Entity
{
    public class SpellManager : IUpdate, ISaveCharacter
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Player player;

        private readonly Dictionary<uint /*spell4BaseId*/, UnlockedSpell> spells = new Dictionary<uint, UnlockedSpell>();
        private readonly Dictionary<uint /*spell4Id*/, double /*cooldown*/> spellCooldowns = new Dictionary<uint, double>();
        private double globalSpellCooldown;

        private ActionSet[] actionSets = new ActionSet[ActionSet.MaxActionSets];
        public byte activeActionSet { get; private set; }

        private PlayerSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="SpellManager"/> from existing <see cref="Character"/> database model.
        /// </summary>
        public SpellManager(Player owner, Character model)
        {
            player = owner;
            for (byte i = 0; i < ActionSet.MaxActionSets; i++)
            {
                actionSets[i] = new ActionSet(i, player);
                foreach(var characterAction in model.CharacterAction
                    .Where(c => c.SpecIndex == i))
                {
                    AddSpell(characterAction.Action, characterAction.TierIndex);
                    var spell = GetSpell(characterAction.Action);
                    actionSets[i].AddSpell(spell, (UILocation)characterAction.Location, characterAction.TierIndex, true);
                }

                foreach(var characterAMP in model.CharacterAMP
                    .Where(c => c.SpecIndex == i))
                    actionSets[i].AddAMP(characterAMP.AMPId, true);
            }
        }

        public void Update(double lastTick)
        {
            // update global cooldown
            if (globalSpellCooldown > 0d)
            {
                if (globalSpellCooldown - lastTick <= 0d)
                {
                    globalSpellCooldown = 0d;
                    log.Trace("Global spell cooldown has reset.");
                }
                else
                    globalSpellCooldown -= lastTick;
            }

            // update spell cooldowns
            foreach ((uint spellId, double cooldown) in spellCooldowns.ToArray())
            {
                if (cooldown - lastTick <= 0d)
                {
                    spellCooldowns.Remove(spellId);
                    log.Trace($"Spell {spellId} cooldown has reset.");
                }
                else
                    spellCooldowns[spellId] = cooldown - lastTick;
            }
        }

        /// <summary>
        /// Add a new <see cref="UnlockedSpell"/> created from supplied spell base id and tier.
        /// </summary>
        public void AddSpell(uint spell4BaseId, byte tier = 1)
        {
            SpellBaseInfo spellBaseInfo = GlobalSpellManager.GetSpellBaseInfo(spell4BaseId);
            if (spellBaseInfo == null)
                throw new ArgumentOutOfRangeException();

            SpellInfo spellInfo = spellBaseInfo.GetSpellInfo(tier);
            if (spellInfo == null)
                throw new ArgumentOutOfRangeException();

            if (spells.ContainsKey(spell4BaseId))
                return;
                //throw new InvalidOperationException();

            Item item = player.Inventory.SpellCreate(spellBaseInfo.Entry, 49);

            var unlockedSpell = new UnlockedSpell(spellBaseInfo, tier, item);
            if (!player.IsLoading)
            {
                player.Session.EnqueueMessageEncrypted(new ServerSpellUpdate
                {
                    Spell4BaseId = spell4BaseId,
                    TierIndex    = tier,
                    Activated    = true
                });
            }

            spells.Add(spellBaseInfo.Entry.Id, unlockedSpell);
        }

         public void UpdateSpell(uint spell4BaseId, byte tier, byte actionSet)
         {
            SpellBaseInfo spellBaseInfo = GlobalSpellManager.GetSpellBaseInfo(spell4BaseId);
            if (spellBaseInfo == null)
                throw new ArgumentOutOfRangeException();

            SpellInfo spellInfo = spellBaseInfo.GetSpellInfo(tier);
            if (spellInfo == null)
                throw new ArgumentOutOfRangeException();

            if (spells.ContainsKey(spell4BaseId))
            {
                spells.Remove(spell4BaseId);
            }

            Item item = player.Inventory.GetSpell(spellBaseInfo.Entry);

            var unlockedSpell = new UnlockedSpell(spellBaseInfo, tier, item);
            if (!player.IsLoading)
            {
                player.Session.EnqueueMessageEncrypted(new ServerSpellUpdate
                {
                    Spell4BaseId = spell4BaseId,
                    TierIndex    = tier,
                    Activated    = tier > 0 ? true : false
                });
            }

            spells.Add(spellBaseInfo.Entry.Id, unlockedSpell);

         }

        /// <summary>
        /// Returns <see cref="UnlockedSpell"/> for an existing spell.
        /// </summary>
        public UnlockedSpell GetSpell(uint spell4BaseId)
        {
            return spells.TryGetValue(spell4BaseId, out UnlockedSpell spell) ? spell : null;
        }

        public List<UnlockedSpell> GetPets()
        {
            return spells.Values
                .Where(s => s.Info.SpellType.Id == 27 ||
                            s.Info.SpellType.Id == 30 ||
                            s.Info.SpellType.Id == 104)
                .ToList();
        }

        /// <summary>
        /// Return spell cooldown for supplied spell id in seconds.
        /// </summary>
        public double GetSpellCooldown(uint spellId)
        {
            return spellCooldowns.TryGetValue(spellId, out double cooldown) ? cooldown : 0d;
        }

        /// <summary>
        /// Set spell cooldown in seconds for supplied spell id.
        /// </summary>
        public void SetSpellCooldown(uint spell4Id, double cooldown)
        {
            if (cooldown < 0d)
                throw new ArgumentOutOfRangeException();

            if (spellCooldowns.ContainsKey(spell4Id))
                spellCooldowns[spell4Id] = cooldown;
            else
                spellCooldowns.Add(spell4Id, cooldown);

            log.Trace($"Spell {spell4Id} cooldown set to {cooldown} seconds.");

            if (!player.IsLoading)
            {
                player.Session.EnqueueMessageEncrypted(new ServerCooldown
                {
                    Cooldown = new Cooldown
                    {
                        Type          = 1,
                        TypeId        = spell4Id,
                        SpellId       = spell4Id,
                        TimeRemaining = (uint)(cooldown * 1000u)
                    }
                });
            }
        }

        public void ResetAllSpellCooldowns()
        {
            foreach (uint spell4Id in spellCooldowns.Keys.ToList())
                SetSpellCooldown(spell4Id, 0d);
        }

        public double GetGlobalSpellCooldown()
        {
            return globalSpellCooldown;
        }

        public void SetGlobalSpellCooldown(double cooldown)
        {
            globalSpellCooldown = cooldown;
            log.Trace($"Global spell cooldown set to {cooldown} seconds.");
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
        
        public void UpdateActionSetSpellTier(byte actionSetIndex, uint spell4BaseId, byte tier)
        {
            if (actionSetIndex >= ActionSet.MaxActionSets)
                throw new ArgumentOutOfRangeException();

            if (!spells.TryGetValue(spell4BaseId, out UnlockedSpell spell))
                throw new ArgumentOutOfRangeException();

            UpdateSpell(spell4BaseId, tier, actionSetIndex);

            actionSets[actionSetIndex].UpdateSpellTier(spell, tier);
        }
        
        public void RemoveSpellFromActionSet(byte actionSetIndex, UILocation location)
        {
            if (actionSetIndex >= ActionSet.MaxActionSets)
                throw new ArgumentOutOfRangeException();

            actionSets[actionSetIndex].RemoveSpell(location);
        }

        public ActionSetAction GetSpellFromActionSet(byte actionSetIndex, UILocation location)
        {
            if (actionSetIndex >= ActionSet.MaxActionSets)
                throw new ArgumentOutOfRangeException();

            return actionSets[actionSetIndex].GetSpell(location);
        }


        public void SendInitialPackets()
        {
            SendServerAbilities();
            SendServerSpellList();
            SendServerAbilityPoints();
            SendServerActionSets();
            SendServerAMPLists();

            player.Session.EnqueueMessageEncrypted(new ServerCooldownList
            {
                Cooldowns = spellCooldowns.Select(c => new Cooldown
                {
                    Type          = 1,
                    SpellId       = c.Key,
                    TypeId        = c.Key,
                    TimeRemaining = (uint)(c.Value * 1000u)
                }).ToList()
            });
        }

        public void SendServerAbilities()
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
        }

        public void SendServerSpellList()
        {
            var serverSpellList = new ServerSpellList();
            foreach ((uint spell4BaseId, UnlockedSpell spell) in spells)
            {
                SpellBaseInfo spellBaseInfo = GlobalSpellManager.GetSpellBaseInfo(spell4BaseId);
                if (spellBaseInfo == null)
                    continue;
                
                // like it or not, this is what retail does
                for (byte i = 0; i < ActionSet.MaxActionSets; i++)
                {
                    ActionSetAction action = actionSets[i].Actions.FirstOrDefault(a => a.ObjectId == spell4BaseId && (a.saveMask & ActionSaveMask.Delete) == 0);
                    if (action != null)
                    {
                        serverSpellList.Spells.Add(new ServerSpellList.Spell
                        {
                            Spell4BaseId      = action.ObjectId,
                            TierIndexAchieved = action.Tier,
                            SpecIndex         = i
                        });
                    } else
                    {
                        serverSpellList.Spells.Add(new ServerSpellList.Spell
                        {
                            Spell4BaseId      = spell4BaseId,
                            TierIndexAchieved = spell.Tier,
                            SpecIndex         = i
                        });
                    }

                    // class ability
                    if (spellBaseInfo.SpellType.Id != 5)
                        break;
                }
            }

            player.Session.EnqueueMessageEncrypted(serverSpellList);
        }

        public void SendServerAMPLists()
        {
            for (byte i = 0; i < ActionSet.MaxActionSets; i++)
                SendServerAMPList(i);
        }

        public void SendServerAMPList(byte actionSetIndex)
        {
            var serverAMPList = new ServerAMPList
            {
                SpecIndex = actionSetIndex
            };

            foreach (var amp in actionSets[actionSetIndex].AMPs
                .Where(a => (a.saveMask & AMPSaveMask.Delete) == 0 && (a.saveMask & AMPSaveMask.Clear) == 0))
                serverAMPList.AMPs.Add(amp.Id);

            player.Session.EnqueueMessageEncrypted(serverAMPList);
        }

        public void UpdateActionSetAMPs(byte actionSetIndex, List<ushort> amps)
        {
            actionSets[activeActionSet].ClearAMPs();
            foreach(ushort amp in amps)
                actionSets[activeActionSet].AddAMP(amp);
        }

        public void RemoveActionSetAMP(byte actionSetIndex,uint amp)
        {
            actionSets[activeActionSet].RemoveAMP((ushort)amp);
        }

        public void ResetActionSetAMPCategory(byte actionSetIndex, uint category)
        {
            var eldanAugmentation = new EldanAugmentationEntry();

            foreach(var amp in actionSets[activeActionSet].AMPs.ToList())
            {
                eldanAugmentation = GameTableManager.EldanAugmentation.GetEntry(amp.Id);
                if (eldanAugmentation == null)
                    continue;

                if (eldanAugmentation.EldanAugmentationCategoryId == category)
                    actionSets[activeActionSet].RemoveAMP(amp.Id);
            }
        }

        public void SendServerAbilityPoints()
        {
            player.Session.EnqueueMessageEncrypted(new ServerAbilityPoints
            {
                AbilityPoints      = actionSets[activeActionSet].TierPoints,
                TotalAbilityPoints = ActionSet.MaxTierPoints
            });
        }

        public void SendServerActionSets()
        {
            for (byte i = 0; i < ActionSet.MaxActionSets; i++)
                SendServerActionSet(i);
        }

        public void SendServerActionSet(byte actionSetsIndex)
        {
            var actionSet = actionSets[actionSetsIndex];
            var serverActionSet = new ServerActionSet
            {
                Index    = actionSet.Index,
                Unknown3 = 1,
                Unknown5 = 1
            };

            for (byte i = 0; i < ActionSet.MaxActionCount; i++)
            {
                ActionSetAction action = actionSet.FirstOrDefault(a => a.Location == (UILocation)i && (a.saveMask & ActionSaveMask.Delete) == 0);
                if (action != null)
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
                else
                {
                    serverActionSet.Actions.Add(new ServerActionSet.Action
                    {
                        ShortcutType = 0,
                        ObjectId     = 0,
                        Location     = new ItemLocation
                        {
                            Location = (InventoryLocation)300, // no idea why 300, this is what retail did
                            BagIndex = i
                        }
                    });
                }
            }

            player.Session.EnqueueMessageEncrypted(serverActionSet);
        }
        
        public uint SetActiveActionSet(byte value)
        {
            var oldValue = activeActionSet;
            uint error = 0;

            if (value < ActionSet.MaxActionSets && value >= 0)
                activeActionSet = value;
            else
                error = 1;

            if (oldValue != activeActionSet)
                saveMask |= PlayerSaveMask.ActiveActionSet;

            return error;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask != PlayerSaveMask.None)
            {
                var model = new Character
                {
                    Id = player.CharacterId
                };

                EntityEntry<Character> entity = context.Attach(model);
                if ((saveMask & PlayerSaveMask.ActiveActionSet) != 0)
                {
                    model.ActiveSpec = activeActionSet;
                    entity.Property(p => p.ActiveSpec).IsModified = true;
                }

                saveMask = PlayerSaveMask.None;
            }

            foreach (var actionSet in actionSets)
                actionSet.Save(context);
        }
    }
}

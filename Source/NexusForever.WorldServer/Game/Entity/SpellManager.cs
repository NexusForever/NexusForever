using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Prerequisite;
using NexusForever.WorldServer.Game.Spell;
using NexusForever.WorldServer.Game.Spell.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NexusForever.WorldServer.Network.Message.Static;
using NLog;

namespace NexusForever.WorldServer.Game.Entity
{
    public class SpellManager : IUpdate, ISaveCharacter
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Index of the active <see cref="ActionSet"/>.
        /// </summary>
        public byte ActiveActionSet
        {
            get => activeActionSet;
            private set
            {
                saveMask |= SpellManagerSaveMask.ActiveActionSet;
                activeActionSet = value;
            }
        }
        private byte activeActionSet;

        public byte InnateIndex
        {
            get => innateIndex;
            private set
            {
                saveMask |= SpellManagerSaveMask.Innate;
                innateIndex = value;
            }
        }
        private byte innateIndex;

        private readonly Player player;

        private readonly Dictionary<uint /*spell4BaseId*/, CharacterSpell> spells = new();
        private readonly Dictionary<uint /*spell4Id*/, double /*cooldown*/> spellCooldowns = new();
        private double globalSpellCooldown;

        private readonly ActionSet[] actionSets = new ActionSet[ActionSet.MaxActionSets];

        private SpellManagerSaveMask saveMask;

        private readonly Dictionary<Class, List<uint> /* spell4Id*/> classPassives = new Dictionary<Class, List<uint>>
        {
            { Class.Warrior, new List<uint> { 46707 } },
            { Class.Engineer, new List<uint> { 58450 } },
            { Class.Esper, new List<uint> { 75261 } },
            { Class.Medic, new List<uint> { 52081 } },
            { Class.Stalker, new List<uint> { 70634 } },
            { Class.Spellslinger, new List<uint> { 69704 } }
        };

        /// <summary>
        /// Create a new <see cref="SpellManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public SpellManager(Player owner, CharacterModel model)
        {
            player = owner;

            foreach (CharacterSpellModel spellModel in model.Spell)
            {
                SpellBaseInfo spellBaseInfo = GlobalSpellManager.Instance.GetSpellBaseInfo(spellModel.Spell4BaseId);
                Item item = player.Inventory.SpellCreate(spellBaseInfo.Entry, ItemUpdateReason.NoReason);
                spells.Add(spellModel.Spell4BaseId, new CharacterSpell(owner, spellModel, spellBaseInfo, item));
            }

            GrantSpells();

            for (byte i = 0; i < ActionSet.MaxActionSets; i++)
            {
                actionSets[i] = new ActionSet(i, player);

                foreach (CharacterActionSetShortcutModel shortcutModel in model.ActionSetShortcut
                    .Where(c => c.SpecIndex == i))
                    actionSets[i].AddShortcut(shortcutModel);

                foreach (CharacterActionSetAmpModel ampModel in model.ActionSetAmp
                    .Where(c => c.SpecIndex == i))
                    actionSets[i].AddAmp(ampModel);
            }

            activeActionSet = model.ActiveSpec;
            innateIndex = model.InnateIndex;
        }

        public void GrantSpells()
        {
            // TODO: TEMPORARY, this should eventually be used on level up
            foreach (SpellLevelEntry spellLevel in GameTableManager.Instance.SpellLevel.Entries
                .Where(s => s.ClassId == (byte)player.Class && s.CharacterLevel <= player.Level)
                .OrderBy(s => s.CharacterLevel))
            {
                //FIXME
                if (spellLevel.PrerequisiteId > 0)
                    continue;

                Spell4Entry spell4Entry = GameTableManager.Instance.Spell4.GetEntry(spellLevel.Spell4Id);
                if (spell4Entry == null)
                    continue;

                if (GetSpell(spell4Entry.Spell4BaseIdBaseSpell) == null)
                    AddSpell(spell4Entry.Spell4BaseIdBaseSpell);
            }

            ClassEntry classEntry = GameTableManager.Instance.Class.GetEntry((byte)player.Class);
            foreach (uint classSpell in classEntry.Spell4IdInnateAbilityActive
                .Concat(classEntry.Spell4IdInnateAbilityPassive)
                .Concat(classEntry.Spell4IdAttackPrimary)
                .Concat(classEntry.Spell4IdAttackUnarmed))
            {
                Spell4Entry spell4Entry = GameTableManager.Instance.Spell4.GetEntry(classSpell);
                if (spell4Entry == null)
                    continue;

                if (GetSpell(spell4Entry.Spell4BaseIdBaseSpell) == null)
                    AddSpell(spell4Entry.Spell4BaseIdBaseSpell);
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

            foreach (CharacterSpell unlockedSpell in spells.Values)
                unlockedSpell.Update(lastTick);
        }

        public void Save(CharacterContext context)
        {
            if (saveMask != SpellManagerSaveMask.None)
            {
                // character is attached in Player::Save, this will only be local lookup
                CharacterModel character = context.Character.Find(player.CharacterId);
                EntityEntry<CharacterModel> entity = context.Entry(character);

                if ((saveMask & SpellManagerSaveMask.ActiveActionSet) != 0)
                {
                    character.ActiveSpec = ActiveActionSet;
                    entity.Property(p => p.ActiveSpec).IsModified = true;
                }

                if ((saveMask & SpellManagerSaveMask.Innate) != 0)
                {
                    character.InnateIndex = InnateIndex;
                    entity.Property(p => p.InnateIndex).IsModified = true;
                }

                saveMask = SpellManagerSaveMask.None;
            }

            foreach (CharacterSpell spell in spells.Values)
                spell.Save(context);

            foreach (ActionSet actionSet in actionSets)
                actionSet.Save(context);
        }

        /// <summary>
        /// Returns <see cref="CharacterSpell"/> for an existing spell.
        /// </summary>
        public CharacterSpell GetSpell(uint spell4BaseId)
        {
            return spells.TryGetValue(spell4BaseId, out CharacterSpell spell) ? spell : null;
        }

        /// <summary>
        /// Add a new <see cref="CharacterSpell"/> created from supplied spell base id and tier.
        /// </summary>
        public void AddSpell(uint spell4BaseId, byte tier = 1)
        {
            SpellBaseInfo spellBaseInfo = GlobalSpellManager.Instance.GetSpellBaseInfo(spell4BaseId);
            if (spellBaseInfo == null)
                throw new ArgumentOutOfRangeException();

            SpellInfo spellInfo = spellBaseInfo.GetSpellInfo(tier);
            if (spellInfo == null)
                throw new ArgumentOutOfRangeException();

            if (spells.ContainsKey(spell4BaseId))
                throw new InvalidOperationException();

            Item item = player.Inventory.SpellCreate(spellBaseInfo.Entry, ItemUpdateReason.NoReason);

            var unlockedSpell = new CharacterSpell(player, spellBaseInfo, tier, item);
            if (!player.IsLoading)
            {
                player.Session.EnqueueMessageEncrypted(new ServerSpellUpdate
                {
                    Spell4BaseId = spell4BaseId,
                    TierIndex    = tier,
                    Activated    = tier > 0
                });
            }

            spells.Add(spellBaseInfo.Entry.Id, unlockedSpell);
        }

        /// <summary>
        /// Update existing <see cref="CharacterSpell"/> with supplied tier. The base tier will be updated if no action set index is supplied.
        /// </summary>
        public void UpdateSpell(uint spell4BaseId, byte tier, byte? actionSetIndex)
        {
            SpellBaseInfo spellBaseInfo = GlobalSpellManager.Instance.GetSpellBaseInfo(spell4BaseId);
            if (spellBaseInfo == null)
                throw new ArgumentOutOfRangeException();

            SpellInfo spellInfo = spellBaseInfo.GetSpellInfo(tier);
            if (spellInfo == null)
                throw new ArgumentOutOfRangeException();

            CharacterSpell spell = GetSpell(spell4BaseId);
            if (spell == null)
                throw new ArgumentOutOfRangeException();

            if (actionSetIndex == null)
                spell.Tier = tier;
            else
            {
                ActionSet actionSet = GetActionSet(actionSetIndex.Value);
                actionSet.UpdateSpellShortcut(spell4BaseId, tier);
            }

            if (!player.IsLoading)
            {
                player.Session.EnqueueMessageEncrypted(new ServerSpellUpdate
                {
                    Spell4BaseId = spell4BaseId,
                    TierIndex = tier,
                    SpecIndex = actionSetIndex ?? 0,
                    Activated = tier > 0
                });
            }
        }

        /// <summary>
        /// Return the tier for supplied spell.
        /// This will either be the <see cref="ActionSetShortcut"/> tier if placed in the active <see cref="ActionSet"/> or base tier if not.
        /// </summary>
        public byte GetSpellTier(uint spell4BaseId)
        {
            CharacterSpell spell = GetSpell(spell4BaseId);
            if (spell == null)
                throw new ArgumentException();

            ActionSet actionSet = GetActionSet(ActiveActionSet);
            ActionSetShortcut shortcut = actionSet.GetShortcut(ShortcutType.Spell, spell4BaseId);
            return shortcut?.Tier ?? spell.Tier;
        }

        public List<CharacterSpell> GetPets()
        {
            return spells.Values
                .Where(s => s.BaseInfo.SpellType.Id == 27 ||
                            s.BaseInfo.SpellType.Id == 30 ||
                            s.BaseInfo.SpellType.Id == 104)
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
        /// Return <see cref="ActionSet"/> at supplied index.
        /// </summary>
        public ActionSet GetActionSet(byte actionSetIndex)
        {
            if (actionSetIndex >= ActionSet.MaxActionSets)
                throw new ArgumentOutOfRangeException();

            return actionSets[actionSetIndex];
        }

        /// <summary>
        /// Update active <see cref="ActionSet"/> with supplied index, returned <see cref="SpecError"/> is sent to the client.
        /// </summary>
        public SpecError SetActiveActionSet(byte value)
        {
            if (value >= ActionSet.MaxActionSets)
                return SpecError.InvalidIndex;

            if (value == ActiveActionSet)
                return SpecError.NoChange;

            // TODO: handle other errors

            ActiveActionSet = value;
            return SpecError.Ok;
        }

        /// <summary>
        /// Update active Innate Ability with supplied index.
        /// </summary>
        public void SetInnate(byte index)
        {
            ClassEntry entry = GameTableManager.Instance.Class.GetEntry((ulong)player.Class);
            if (entry == null)
                throw new InvalidOperationException($"Player Class does not have an entry: {player.Class}");

            if (entry.Spell4IdInnateAbilityActive[index] == 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            uint innateActiveBaseId = GameTableManager.Instance.Spell4.GetEntry(entry.Spell4IdInnateAbilityActive[index]).Spell4BaseIdBaseSpell;
            if (innateActiveBaseId == 0)
                throw new InvalidOperationException("Selected Innate ability does not have an associated Spell4 Entry.");

            CharacterSpell innateActive = GetSpell(innateActiveBaseId);
            if (innateActive == null)
                throw new InvalidOperationException($"Player does not have spell with a Base ID of {innateActiveBaseId}");

            if (entry.PrerequisiteIdInnateAbility[index] > 0)
                if (!PrerequisiteManager.Instance.Meets(player, entry.PrerequisiteIdInnateAbility[index]))
                    return;

            CastInnatePassive(index);

            InnateIndex = index;
        }

        private void CastInnatePassive(int newInnate = -1)
        {
            uint innateIndexToCast = newInnate > -1 ? (uint)newInnate : InnateIndex;

            ClassEntry entry = GameTableManager.Instance.Class.GetEntry((ulong)player.Class);
            if (entry == null)
                throw new InvalidOperationException($"Player Class does not have an entry: {player.Class}");

            if (newInnate > -1)
            {
                // TODO: End existing Innate Spell
                uint oldInnatePassiveSpell4Id = entry.Spell4IdInnateAbilityPassive[innateIndex];
                if (player.HasSpell(oldInnatePassiveSpell4Id, out Spell.Spell oldInnatePassive))
                    oldInnatePassive.Finish();
            }

            uint innatePassiveSpell4Id = entry.Spell4IdInnateAbilityPassive[innateIndexToCast];
            if (innatePassiveSpell4Id != 0)
            {
                if (player.HasSpell(innatePassiveSpell4Id, out Spell.Spell currentPassiveBuff))
                    return;

                CharacterSpell passiveSpell = GetSpell(GameTableManager.Instance.Spell4.GetEntry(entry.Spell4IdInnateAbilityActive[innateIndexToCast]).Spell4BaseIdBaseSpell);
                if (passiveSpell == null)
                    throw new InvalidOperationException($"Selected Innate Passive does not have an associated Spell4 Entry (ID: {innatePassiveSpell4Id})");

                player.CastSpell(innatePassiveSpell4Id, new SpellParameters
                {
                    UserInitiatedSpellCast = false
                });
            }
        }

        /// <summary>
        /// This uses passive On Start abilities that were caught in sniffs. Provides a lot of Procs and hidden class functionality.
        /// </summary>
        private void CastClassPassives()
        {
            if (classPassives.TryGetValue(player.Class, out List<uint> classPassiveSpells))
            {
                foreach (uint spell4Id in classPassiveSpells)
                {
                    if (player.HasSpell(spell4Id, out Spell.Spell currentClassPassive))
                        continue;

                    player.CastSpell(spell4Id, new SpellParameters
                    {
                        UserInitiatedSpellCast = false
                    });
                }
            }
        }

        public void SendInitialPackets()
        {
            SendServerAbilities();
            SendServerSpellList();
            SendServerAbilityPoints();
            SendServerActionSets();
            SendServerAmpLists();
            SendServerPlayerInnate();
            CastInnatePassive();
            CastClassPassives();

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

        private void SendServerAbilities()
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
                        Reason = ItemUpdateReason.NoReason
                    }
                });
            }
        }

        private void SendServerSpellList()
        {
            var serverSpellList = new ServerSpellList();
            foreach ((uint spell4BaseId, CharacterSpell spell) in spells)
            {
                SpellBaseInfo spellBaseInfo = GlobalSpellManager.Instance.GetSpellBaseInfo(spell4BaseId);
                if (spellBaseInfo == null)
                    continue;
                
                for (byte i = 0; i < ActionSet.MaxActionSets; i++)
                {
                    ActionSetShortcut shortcut = actionSets[i].GetShortcut(ShortcutType.Spell, spell4BaseId);
                    serverSpellList.Spells.Add(new ServerSpellList.Spell
                    {
                        Spell4BaseId      = spell4BaseId,
                        TierIndexAchieved = shortcut?.Tier ?? spell.Tier,
                        SpecIndex         = i
                    });

                    // class ability
                    if (spellBaseInfo.SpellType.Id != 5)
                        break;
                }
            }

            player.Session.EnqueueMessageEncrypted(serverSpellList);
        }

        public void SendServerAbilityPoints()
        {
            player.Session.EnqueueMessageEncrypted(new ServerAbilityPoints
            {
                AbilityPoints      = actionSets[ActiveActionSet].TierPoints,
                TotalAbilityPoints = ActionSet.MaxTierPoints
            });
        }

        private void SendServerActionSets()
        {
            for (byte i = 0; i < ActionSet.MaxActionSets; i++)
            {
                ActionSet actionSet = GetActionSet(i);
                player.Session.EnqueueMessageEncrypted(actionSet.BuildServerActionSet());
            }
        }

        private void SendServerAmpLists()
        {
            for (byte i = 0; i < ActionSet.MaxActionSets; i++)
            {
                ActionSet actionSet = GetActionSet(i);
                player.Session.EnqueueMessageEncrypted(actionSet.BuildServerAmpList());
            }
        }

        private void SendServerPlayerInnate()
        {
            player.Session.EnqueueMessageEncrypted(new ServerPlayerInnate
            {
                InnateIndex = InnateIndex
            });
        }
    }
}

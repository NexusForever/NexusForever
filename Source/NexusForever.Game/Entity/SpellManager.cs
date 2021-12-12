using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Prerequisite;
using NexusForever.Game.Spell;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Network.World.Message.Static;
using NLog;

namespace NexusForever.Game.Entity
{
    public class SpellManager : ISpellManager
    {
        /// <summary>
        /// Determines which fields need saving for <see cref="ISpellManager"/> when being saved to the database.
        /// </summary>
        [Flags]
        public enum SpellManagerSaveMask
        {
            None            = 0x0000,
            ActiveActionSet = 0x0001,
            Innate          = 0x0002
        }

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Index of the active <see cref="IActionSet"/>.
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
            set
            {
                innateIndex = value;
                saveMask |= SpellManagerSaveMask.Innate;
            }
        }
        private byte innateIndex;

        private readonly IPlayer player;
        private ICharacterSpell continuousSpell;

        private readonly Dictionary<uint /*spell4BaseId*/, ICharacterSpell> spells = new();
        private readonly Dictionary<uint /*spell4Id*/, double /*cooldown*/> spellCooldowns = new();
        private readonly Dictionary<uint /*cooldownId*/, double /*cooldown*/> cooldownIds = new();
        private readonly Dictionary<uint /*globalCooldownEnum*/, double /*cooldown*/> globalSpellCooldowns = new();
        private uint maxGlobalSpellCooldownEnum = 3; // TODO: Read value from GameTables?

        private readonly IActionSet[] actionSets = new ActionSet[ActionSet.MaxActionSets];

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
        /// Create a new <see cref="ISpellManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public SpellManager(IPlayer owner, CharacterModel model)
        {
            player = owner;

            foreach (CharacterSpellModel spellModel in model.Spell)
            {
                ISpellBaseInfo spellBaseInfo = GlobalSpellManager.Instance.GetSpellBaseInfo(spellModel.Spell4BaseId);
                IItem item = player.Inventory.SpellCreate(spellBaseInfo.Entry, ItemUpdateReason.NoReason);
                spells.Add(spellModel.Spell4BaseId, new CharacterSpell(owner, spellModel, spellBaseInfo, item));
            }

            for (uint i = 0; i <= maxGlobalSpellCooldownEnum; i++)
                globalSpellCooldowns.Add(i, 0d);

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
            innateIndex     = model.InnateIndex;
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

                // Add Pet Abilities if this Spell has a Pet summoned from it
                if (spell4Entry.Spell4IdPetSwitch > 0)
                {
                    Spell4Entry spell4PetEntry = GameTableManager.Instance.Spell4.GetEntry(spell4Entry.Spell4IdPetSwitch);
                    if (spell4PetEntry == null)
                        continue;

                    if (GetSpell(spell4PetEntry.Spell4BaseIdBaseSpell) == null)
                        AddSpell(spell4PetEntry.Spell4BaseIdBaseSpell);
                }

                if (spell4Entry.Spell4IdMechanicAlternateSpell > 0)
                {
                    Spell4Entry spell4AltEntry = GameTableManager.Instance.Spell4.GetEntry(spell4Entry.Spell4IdMechanicAlternateSpell);
                    if (spell4AltEntry == null)
                        continue;

                    if (GetSpell(spell4AltEntry.Spell4BaseIdBaseSpell) == null)
                        AddSpell(spell4AltEntry.Spell4BaseIdBaseSpell);
                }
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
            // update global cooldowns
            foreach ((uint globalEnum, double cooldown) in globalSpellCooldowns.ToArray())
            {
                if (cooldown <= 0d)
                    continue;

                globalSpellCooldowns[globalEnum] = cooldown - lastTick;
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

            // update cooldown id groups
            foreach ((uint cooldownId, double cooldown) in cooldownIds.ToArray())
            {
                if (cooldown - lastTick <= 0d)
                {
                    cooldownIds.Remove(cooldownId);
                    log.Trace($"Cooldown ID {cooldownId} has reset.");
                }
                else
                    cooldownIds[cooldownId] = cooldown - lastTick;
            }

            foreach (CharacterSpell unlockedSpell in spells.Values)
                unlockedSpell.Update(lastTick);

            if (continuousSpell != null && globalSpellCooldowns[continuousSpell.GlobalCooldownEnum] <= 0d && !player.IsCasting())
                continuousSpell?.SpellManagerCast();
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

            foreach (ICharacterSpell spell in spells.Values)
                spell.Save(context);

            foreach (IActionSet actionSet in actionSets)
                actionSet.Save(context);
        }

        /// <summary>
        /// Returns <see cref="ICharacterSpell"/> for an existing spell.
        /// </summary>
        public ICharacterSpell GetSpell(uint spell4BaseId)
        {
            return spells.TryGetValue(spell4BaseId, out ICharacterSpell spell) ? spell : null;
        }

        /// <summary>
        /// Add a new <see cref="ICharacterSpell"/> created from supplied spell base id and tier.
        /// </summary>
        public void AddSpell(uint spell4BaseId, byte tier = 1)
        {
            ISpellBaseInfo spellBaseInfo = GlobalSpellManager.Instance.GetSpellBaseInfo(spell4BaseId);
            if (spellBaseInfo == null)
                throw new ArgumentOutOfRangeException();

            ISpellInfo spellInfo = spellBaseInfo.GetSpellInfo(tier);
            if (spellInfo == null)
                throw new ArgumentOutOfRangeException();

            if (spells.ContainsKey(spell4BaseId))
                throw new InvalidOperationException();

            IItem item = player.Inventory.SpellCreate(spellBaseInfo.Entry, ItemUpdateReason.NoReason);

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
        /// Update existing <see cref="ICharacterSpell"/> with supplied tier. The base tier will be updated if no action set index is supplied.
        /// </summary>
        public void UpdateSpell(uint spell4BaseId, byte tier, byte? actionSetIndex)
        {
            ISpellBaseInfo spellBaseInfo = GlobalSpellManager.Instance.GetSpellBaseInfo(spell4BaseId);
            if (spellBaseInfo == null)
                throw new ArgumentOutOfRangeException();

            ISpellInfo spellInfo = spellBaseInfo.GetSpellInfo(tier);
            if (spellInfo == null)
                throw new ArgumentOutOfRangeException();

            ICharacterSpell spell = GetSpell(spell4BaseId);
            if (spell == null)
                throw new ArgumentOutOfRangeException();

            if (actionSetIndex == null)
                spell.Tier = tier;
            else
            {
                IActionSet actionSet = GetActionSet(actionSetIndex.Value);
                actionSet.UpdateSpellShortcut(spell4BaseId, tier);
            }

            if (!player.IsLoading)
            {
                player.Session.EnqueueMessageEncrypted(new ServerSpellUpdate
                {
                    Spell4BaseId = spell4BaseId,
                    TierIndex    = tier,
                    SpecIndex    = actionSetIndex ?? 0,
                    Activated    = tier > 0
                });
            }
        }

        /// <summary>
        /// Return the tier for supplied spell.
        /// This will either be the <see cref="IActionSetShortcut"/> tier if placed in the active <see cref="IActionSet"/> or base tier if not.
        /// </summary>
        public byte GetSpellTier(uint spell4BaseId)
        {
            ICharacterSpell spell = GetSpell(spell4BaseId);
            if (spell == null)
                throw new ArgumentException();

            IActionSet actionSet = GetActionSet(ActiveActionSet);
            IActionSetShortcut shortcut = actionSet.GetShortcut(ShortcutType.Spell, spell4BaseId);
            return shortcut?.Tier ?? spell.Tier;
        }

        public List<ICharacterSpell> GetPets()
        {
            return spells.Values
                .Where(s => s.BaseInfo.SpellType.Id == 27 ||
                            s.BaseInfo.SpellType.Id == 30 ||
                            s.BaseInfo.SpellType.Id == 104)
                .ToList();
        }

        /// <summary>
        /// Return remaining cooldown for supplied cooldown ID.
        /// </summary>
        public double GetSpellCooldownByCooldownId(uint spell4CooldownId)
        {
            return cooldownIds.TryGetValue(spell4CooldownId, out double cooldown) ? cooldown : 0u;
        }

        /// <summary>
        /// Update all spell cooldowns that share a given cooldown ID
        /// </summary>
        public void SetSpellCooldownByCooldownId(uint spell4CooldownId, double newCooldown)
        {
            if (newCooldown < 0d)
                throw new ArgumentOutOfRangeException();

            if (cooldownIds.TryGetValue(spell4CooldownId, out double cooldown))
                cooldownIds[spell4CooldownId] = newCooldown;
            else
                cooldownIds.TryAdd(spell4CooldownId, newCooldown);
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
        private void _SetSpellCooldown(uint spell4Id, double cooldown, bool emit)
        {
            if (cooldown < 0d)
                throw new ArgumentOutOfRangeException();

            if (spellCooldowns.ContainsKey(spell4Id))
                spellCooldowns[spell4Id] = cooldown;
            else
                spellCooldowns.Add(spell4Id, cooldown);

            log.Trace($"Spell {spell4Id} cooldown set to {cooldown} seconds.");

            if (!player.IsLoading && emit)
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

        /// <summary>
        /// Set spell cooldown in seconds for supplied <see cref="ISpellInfo"/>.
        /// </summary>
        public void SetSpellCooldown(ISpellInfo spellInfo, double cooldown, bool emit = false)
        {
            if (cooldown < 0d)
                throw new ArgumentOutOfRangeException();

            _SetSpellCooldown(spellInfo.Entry.Id, cooldown, emit);

            foreach (SpellCoolDownEntry coolDownEntry in spellInfo.Cooldowns)
                SetSpellCooldownByCooldownId(coolDownEntry.Id, cooldown);
        }

        /// <summary>
        /// Set spell cooldown in seconds for supplied spell id.
        /// </summary>
        public void SetSpellCooldown(uint spell4Id, double cooldown, bool emit = false)
        {
            if (cooldown < 0d)
                throw new ArgumentOutOfRangeException();

            Spell4Entry entry = GameTableManager.Instance.Spell4.GetEntry(spell4Id);
            if (entry == null)
                throw new InvalidOperationException($"Spell4 with ID ({spell4Id}) does not exist.");

            ISpellBaseInfo baseInfo = GlobalSpellManager.Instance.GetSpellBaseInfo(entry.Spell4BaseIdBaseSpell);
            if (baseInfo == null)
                throw new InvalidOperationException($"BaseInfo with ID ({entry.Spell4BaseIdBaseSpell}) does not exist.");

            SetSpellCooldown(baseInfo.GetSpellInfo((byte)entry.TierIndex), cooldown, emit);
        }

        /// <summary>
        /// Update all spell cooldowns that share a given group ID
        /// </summary>
        public void SetSpellCooldownByGroupId(uint groupId, double cooldown)
        {
            if (cooldown < 0d)
                throw new ArgumentOutOfRangeException();

            foreach (uint spell4 in spellCooldowns.Keys.ToList())
            {
                Spell4Entry spell4Entry = GameTableManager.Instance.Spell4.GetEntry(spell4);
                if (spell4Entry == null)
                    throw new InvalidOperationException($"Spell4 with ID ({spell4}) does not exist.");

                if (spell4Entry.Spell4GroupListId == 0u)
                    continue;

                Spell4GroupListEntry spell4GroupListEntry = GameTableManager.Instance.Spell4GroupList.GetEntry(spell4Entry.Spell4GroupListId);
                if (spell4GroupListEntry == null)
                    throw new InvalidOperationException($"Spell4 Group List with ID ({spell4Entry.Spell4GroupListId}) does not exist.");

                if (spell4GroupListEntry.SpellGroupIds.Contains(groupId))
                    SetSpellCooldown(spell4, cooldown);
            }
        }

        /// <summary>
        /// Update all spell cooldowns that share a base spell ID
        /// </summary>
        public void SetSpellCooldownByBaseSpell(uint spell4BaseId, uint type, double cooldown)
        {
            ISpellBaseInfo baseSpellInfo = GlobalSpellManager.Instance.GetSpellBaseInfo(spell4BaseId);
            if (baseSpellInfo == null)
                throw new ArgumentNullException();

            foreach (ISpellInfo spellInfo in baseSpellInfo)
            {
                if (spellCooldowns.TryGetValue(spellInfo.Entry.Id, out double currentCd))
                {
                    if (type == 3)
                        SetSpellCooldown(spellInfo.Entry.Id, cooldown / 1000d);
                    else
                        SetSpellCooldown(spellInfo.Entry.Id, currentCd * cooldown);
                }
            }
        }

        public void ResetAllSpellCooldowns()
        {
            foreach (uint spell4Id in spellCooldowns.Keys.ToList())
                SetSpellCooldown(spell4Id, 0d, true);
        }

        public double GetGlobalSpellCooldown(uint globalEnum)
        {
            return globalSpellCooldowns[globalEnum];
        }

        public void SetGlobalSpellCooldown(uint globalEnum, double cooldown)
        {
            globalSpellCooldowns[globalEnum] = cooldown;
            log.Trace($"Global spell cooldown {globalEnum} set to {cooldown} seconds.");
        }

        /// <summary>
        /// Return <see cref="IActionSet"/> at supplied index.
        /// </summary>
        public IActionSet GetActionSet(byte actionSetIndex)
        {
            if (actionSetIndex >= ActionSet.MaxActionSets)
                throw new ArgumentOutOfRangeException();

            return actionSets[actionSetIndex];
        }

        /// <summary>
        /// Update active <see cref="IActionSet"/> with supplied index, returned <see cref="SpecError"/> is sent to the client.
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

            ICharacterSpell innateActive = GetSpell(innateActiveBaseId);
            if (innateActive == null)
                throw new InvalidOperationException($"Player does not have spell with a Base ID of {innateActiveBaseId}");

            if (entry.PrerequisiteIdInnateAbility[index] > 0)
                if (!PrerequisiteManager.Instance.Meets(player, entry.PrerequisiteIdInnateAbility[index]))
                    return;

            // TODO: Cast Innate Passives

            InnateIndex = index;
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
                    if (player.HasSpell(spell4Id, out ISpell currentClassPassive))
                        continue;

                    player.CastSpell(spell4Id, new SpellParameters
                    {
                        UserInitiatedSpellCast = false
                    });
                }
            }
        }

        /// <summary>
        /// Set the supplied spell as the spell to Continuously Cast when GCD expires. Should only be called by a <see cref="ICharacterSpell"/>.
        /// </summary>
        public void SetAsContinuousCast(ICharacterSpell spell)
        {
            continuousSpell = spell;
        }

        public void SendInitialPackets()
        {
            SendServerAbilities();
            SendServerSpellList();
            SendServerAbilityPoints();
            SendServerActionSets();
            SendServerAmpLists();
            SendServerPlayerInnate();
            // TODO: Cast Innate Spells
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
            foreach (IItem spell in player.Inventory
                .Where(b => b.Location == InventoryLocation.Ability)
                .SelectMany(s => s))
            {
                player.Session.EnqueueMessageEncrypted(new ServerItemAdd
                {
                    InventoryItem = new InventoryItem
                    {
                        Item   = spell.Build(),
                        Reason = ItemUpdateReason.NoReason
                    }
                });
            }
        }

        private void SendServerSpellList()
        {
            var serverSpellList = new ServerSpellList();
            foreach ((uint spell4BaseId, ICharacterSpell spell) in spells)
            {
                ISpellBaseInfo spellBaseInfo = GlobalSpellManager.Instance.GetSpellBaseInfo(spell4BaseId);
                if (spellBaseInfo == null)
                    continue;

                for (byte i = 0; i < ActionSet.MaxActionSets; i++)
                {
                    IActionSetShortcut shortcut = actionSets[i].GetShortcut(ShortcutType.Spell, spell4BaseId);
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
                IActionSet actionSet = GetActionSet(i);
                player.Session.EnqueueMessageEncrypted(actionSet.BuildServerActionSet());
            }
        }

        private void SendServerAmpLists()
        {
            for (byte i = 0; i < ActionSet.MaxActionSets; i++)
            {
                IActionSet actionSet = GetActionSet(i);
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

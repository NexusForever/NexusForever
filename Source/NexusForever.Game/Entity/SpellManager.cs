using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Info;
using NexusForever.Game.Prerequisite;
using NexusForever.Game.Spell;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Abilities;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Network.World.Message.Model.Spell;
using NexusForever.Network.World.Message.Static;
using NexusForever.Shared;
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
            Innate          = 0x0002,
            BonusAmpPower   = 0x0004
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

        /// <summary>
        /// AMP points granted to this character by <c>AMP Power Upgrade</c> items.
        /// </summary>
        /// <remarks>
        /// The client's own bonus power slot, fed only by <see cref="ServerAmpPowerUpdate"/> and by
        /// the same field of <see cref="ServerPlayerCreate"/>. It is signed on the wire but the
        /// client compares it unsigned, so a negative value fails the cap gate instead of passing
        /// it; the setter keeps it inside 0..<see cref="MaxBonusAmpPower"/>.
        /// </remarks>
        public short BonusAmpPower
        {
            get => bonusAmpPower;
            private set
            {
                bonusAmpPower = value;
                saveMask |= SpellManagerSaveMask.BonusAmpPower;
            }
        }
        private short bonusAmpPower;

        /// <summary>
        /// The size of the client's own unlocked inlaid augmentation array.
        /// </summary>
        public const byte MaxUnlockedInlaidEldanAugmentations = 54;

        /// <summary>
        /// Highest bonus power an <c>AMP Power Upgrade</c> item may grant a character.
        /// </summary>
        /// <remarks>
        /// <c>Prerequisite</c> 28557, the cast gate of the only spell that grants it, is a type 237
        /// (<c>BonusPower</c>) check with comparison id 5, which the client
        /// evaluates as a strict <c>&lt;</c> against its value of 10. So the 11th grant is refused
        /// and the accumulator stops at 10.
        /// </remarks>
        public const short MaxBonusAmpPower = 10;

        private readonly IPlayer player;
        private ICharacterSpell continuousSpell;

        private readonly Dictionary<uint /*spell4BaseId*/, ICharacterSpell> spells = new();
        private readonly Dictionary<uint /*spell4Id*/, double /*cooldown*/> spellCooldowns = new();
        private readonly Dictionary<uint /*cooldownId*/, double /*cooldown*/> cooldownIds = new();
        private readonly Dictionary<uint /*globalCooldownEnum*/, double /*cooldown*/> globalSpellCooldowns = new();
        private uint maxGlobalSpellCooldownEnum = 3; // TODO: Read value from GameTables?

        private readonly IActionSet[] actionSets = new ActionSet[ActionSet.MaxActionSets];

        private readonly HashSet<ushort> unlockedInlaidEldanAugmentations = new();
        private readonly List<ushort> unlockedInlaidEldanAugmentationsToSave = new();

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
                ISpellBaseInfo spellBaseInfo = LegacyServiceProvider.Provider.GetService<ISpellInfoManager>().GetSpellBaseInfo(spellModel.Spell4BaseId);
                IItem item = player.Inventory.SpellCreate(spellBaseInfo.Entry, ItemUpdateReason.NoReason);
                spells.Add(spellModel.Spell4BaseId, new CharacterSpell(owner, spellModel, spellBaseInfo, item));
            }

            for (uint i = 0; i <= maxGlobalSpellCooldownEnum; i++)
                globalSpellCooldowns.Add(i, 0d);

            // the action sets do not exist yet, so the gated rows cannot be evaluated here;
            // SendInitialPackets does that once the player is built
            GrantSpells(evaluatePrerequisites: false);

            // before the action sets are built: their point budget reads it.
            bonusAmpPower = model.BonusAmpPower;

            for (byte i = 0; i < ActionSet.MaxActionSets; i++)
            {
                actionSets[i] = new ActionSet(i, player, this);

                foreach (CharacterActionSetShortcutModel shortcutModel in model.ActionSetShortcut
                    .Where(c => c.SpecIndex == i))
                    actionSets[i].AddShortcut(shortcutModel);

                foreach (CharacterActionSetAmpModel ampModel in model.ActionSetAmp
                    .Where(c => c.SpecIndex == i))
                    actionSets[i].AddAmp(ampModel);
            }

            foreach (CharacterEldanAugmentationUnlockModel unlockModel in model.EldanAugmentationUnlock)
                unlockedInlaidEldanAugmentations.Add(unlockModel.EldanAugmentationId);

            activeActionSet = model.ActiveSpec;
            innateIndex     = model.InnateIndex;
        }

        public void GrantSpells(bool evaluatePrerequisites = true)
        {
            // TODO: TEMPORARY, this should eventually be used on level up
            foreach (SpellLevelEntry spellLevel in GameTableManager.Instance.SpellLevel.Entries
                .Where(s => s.ClassId == (byte)player.Class && s.CharacterLevel <= player.Level)
                .OrderBy(s => s.CharacterLevel))
            {
                // A SpellLevel row can carry a prerequisite that decides whether the ability is
                // granted at all; the AMP unlocked abilities are gated this way (prerequisite type
                // 227, "this Eldan augmentation is selected in the active spec").
                // Still not handled: the rows gated behind the Primal Matrix, prerequisite type 292
                // and the type 11 rows that chain to it. No check exists for type 292, so
                // CanEvaluate rejects those rows and they stay blocked, without the warning that
                // evaluating them would log on every pass.
                // TODO: a spell granted through a prerequisite is never taken away again. When the
                // AMP that unlocked it is deselected, or the player switches to a spec that does not
                // have it, the ability stays in the spellbook. Whether retail removed it or only hid
                // it is unverified, and so is the message the client expects for a removal, so
                // nothing is removed for now.
                if (spellLevel.PrerequisiteId > 0)
                {
                    if (!evaluatePrerequisites)
                        continue;

                    if (!PrerequisiteManager.Instance.CanEvaluate(spellLevel.PrerequisiteId))
                        continue;

                    if (!PrerequisiteManager.Instance.Meets(player, spellLevel.PrerequisiteId))
                        continue;
                }

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

                if ((saveMask & SpellManagerSaveMask.BonusAmpPower) != 0)
                {
                    character.BonusAmpPower = BonusAmpPower;
                    entity.Property(p => p.BonusAmpPower).IsModified = true;
                }

                saveMask = SpellManagerSaveMask.None;
            }

            foreach (ICharacterSpell spell in spells.Values)
                spell.Save(context);

            foreach (IActionSet actionSet in actionSets)
                actionSet.Save(context);

            foreach (ushort eldanAugmentationId in unlockedInlaidEldanAugmentationsToSave)
            {
                context.Add(new CharacterEldanAugmentationUnlockModel
                {
                    Id                  = player.CharacterId,
                    EldanAugmentationId = eldanAugmentationId
                });
            }

            unlockedInlaidEldanAugmentationsToSave.Clear();
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
            ISpellBaseInfo spellBaseInfo = LegacyServiceProvider.Provider.GetService<ISpellInfoManager>().GetSpellBaseInfo(spell4BaseId);
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
            ISpellBaseInfo spellBaseInfo = LegacyServiceProvider.Provider.GetService<ISpellInfoManager>().GetSpellBaseInfo(spell4BaseId);
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
            IActionSetShortcut shortcut = actionSet.GetShortcut(ShortcutType.SpellbookItem, spell4BaseId);
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
                player.Session.EnqueueMessageEncrypted(new ServerSpellCooldownStart
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

            ISpellBaseInfo baseInfo = LegacyServiceProvider.Provider.GetService<ISpellInfoManager>().GetSpellBaseInfo(entry.Spell4BaseIdBaseSpell);
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
            ISpellBaseInfo baseSpellInfo = LegacyServiceProvider.Provider.GetService<ISpellInfoManager>().GetSpellBaseInfo(spell4BaseId);
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

            GetActionSet(ActiveActionSet).FinishAmps();

            ActiveActionSet = value;

            GetActionSet(ActiveActionSet).CastAmps();

            // the new spec's AMP selection is in place, so a SpellLevel row gated behind one of its
            // AMPs can be granted now
            GrantSpells();

            return SpecError.Ok;
        }

        /// <summary>
        /// Returns if the supplied <see cref="EldanAugmentationEntry"/> id is an unlocked inlaid augmentation.
        /// </summary>
        public bool HasUnlockedInlaidEldanAugmentation(ushort eldanAugmentationId)
        {
            return unlockedInlaidEldanAugmentations.Contains(eldanAugmentationId);
        }

        /// <summary>
        /// Unlock the inlaid <see cref="EldanAugmentationEntry"/> with supplied id.
        /// </summary>
        public void UnlockInlaidEldanAugmentation(ushort eldanAugmentationId)
        {
            if (unlockedInlaidEldanAugmentations.Contains(eldanAugmentationId))
                return;

            // the client allocates a fixed 54 entry array for this list and would read past it, so the set
            // must never grow beyond that. 237 augmentations are unlock targets in the shipped data.
            if (unlockedInlaidEldanAugmentations.Count >= MaxUnlockedInlaidEldanAugmentations)
            {
                log.Warn($"Failed to unlock inlaid eldan augmentation {eldanAugmentationId} for character " +
                    $"{player.CharacterId}, the set is already at its {MaxUnlockedInlaidEldanAugmentations} entry limit.");
                return;
            }

            unlockedInlaidEldanAugmentations.Add(eldanAugmentationId);
            unlockedInlaidEldanAugmentationsToSave.Add(eldanAugmentationId);

            SendUnlockedInlaidEldanAugmentations();
        }

        /// <summary>
        /// Send the complete unlocked inlaid augmentation set; the client replaces its own wholesale.
        /// </summary>
        public void SendUnlockedInlaidEldanAugmentations()
        {
            player.Session.EnqueueMessageEncrypted(new ServerUnlockedInlaidEldanAugmentations
            {
                EldanAugmentations = unlockedInlaidEldanAugmentations.ToList()
            });
        }

        /// <summary>
        /// Grant supplied amount of bonus AMP power, clamped to 0..<see cref="MaxBonusAmpPower"/>.
        /// </summary>
        /// <remarks>
        /// The cap itself is enforced before the cast, by <c>Prerequisite</c> 28557 on the granting
        /// spell; this clamp only makes sure no route can push the accumulator outside the range the
        /// client's unsigned comparison relies on.
        /// </remarks>
        public void GrantBonusAmpPower(int amount)
        {
            int granted = BonusAmpPower + amount;

            if (granted < 0)
                granted = 0;
            else if (granted > MaxBonusAmpPower)
                granted = MaxBonusAmpPower;

            if (granted == BonusAmpPower)
                return;

            BonusAmpPower = (short)granted;

            SendAmpPowerUpdate();

            log.Trace($"Character {player.CharacterId} bonus AMP power is now {BonusAmpPower}.");
        }

        /// <summary>
        /// Send the item granted bonus AMP power; the client recomputes its point total from it.
        /// </summary>
        public void SendAmpPowerUpdate()
        {
            player.Session.EnqueueMessageEncrypted(new ServerAmpPowerUpdate
            {
                BonusPower = BonusAmpPower
            });
        }

        /// <summary>
        /// Validate the supplied AMP ids against the <see cref="IActionSet"/> at supplied index.
        /// </summary>
        /// <remarks>
        /// Mirrors the order the client's own AMP configuration validator uses, so that a commit the client
        /// considered legal is not refused here and the other way round. <paramref name="newAmps"/> are the ids
        /// being added; the tier and series rules are evaluated over the resulting selection.
        /// </remarks>
        public LimitedActionSetResult ValidateAmps(byte specIndex, IEnumerable<ushort> newAmps)
        {
            if (player.InCombat)
                return LimitedActionSetResult.InCombat;

            IActionSet actionSet = GetActionSet(specIndex);

            var added = new List<EldanAugmentationEntry>();
            uint powerCost = 0u;

            foreach (ushort id in newAmps)
            {
                EldanAugmentationEntry entry = GameTableManager.Instance.EldanAugmentation.GetEntry(id);
                if (entry == null)
                    return LimitedActionSetResult.EldanAugmentationInvalidId;

                if (actionSet.GetAmp(id) != null || added.Any(e => e.Id == id))
                    continue;

                powerCost += entry.PowerCost;
                if (powerCost > actionSet.AmpPoints)
                    return LimitedActionSetResult.EldanAugmentationNotEnoughPower;

                if (entry.ClassId != (uint)player.Class)
                    return LimitedActionSetResult.UnknownClassId;

                // the client gates an augmentation on the unlocked inlaid set when, and only when, it carries
                // an Item2IdUnlock.
                if (entry.Item2IdUnlock != 0u && !HasUnlockedInlaidEldanAugmentation((ushort)entry.Id))
                    return LimitedActionSetResult.EldanAugmentationLockedInlaid;

                if (GameTableManager.Instance.EldanAugmentationCategory.GetEntry(entry.EldanAugmentationCategoryId) == null)
                    return LimitedActionSetResult.EldanAugmentationInvalidCategoryId;

                if (entry.CategoryTier > 2u)
                    return LimitedActionSetResult.EldanAugmentationInvalidCategoryTier;

                added.Add(entry);
            }

            List<EldanAugmentationEntry> selection = actionSet.Amps
                .Select(a => a.Entry)
                .Concat(added)
                .ToList();

            // every augmentation a selected augmentation requires must itself be selected.
            foreach (EldanAugmentationEntry entry in selection)
            {
                if (entry.EldanAugmentationIdRequired == 0u)
                    continue;

                if (!selection.Any(e => e.Id == entry.EldanAugmentationIdRequired))
                    return LimitedActionSetResult.EldanAugmentationInvalidSeries;
            }

            // a tier is only open once enough power has been spent on lower tiers of the same category.
            foreach (EldanAugmentationEntry entry in selection.Where(e => e.CategoryTier > 0u))
            {
                EldanAugmentationCategoryEntry category = GameTableManager.Instance.EldanAugmentationCategory
                    .GetEntry(entry.EldanAugmentationCategoryId);

                uint required = entry.CategoryTier == 1u ? category.Tier2CostAmount00 : category.Tier3CostAmount00;

                uint spent = (uint)selection
                    .Where(e => e.EldanAugmentationCategoryId == entry.EldanAugmentationCategoryId
                        && e.CategoryTier < entry.CategoryTier)
                    .Sum(e => e.PowerCost);

                if (spent < required)
                    return LimitedActionSetResult.EldanAugmentationLockedCategoryTier;
            }

            return LimitedActionSetResult.Ok;
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
            // the gated rows the constructor left alone can be evaluated now, and this has to run
            // before the ability items and the ability list go out below
            GrantSpells();

            SendServerAbilities();
            SendServerAbilityList();
            SendServerAbilityPoints();
            SendServerActionSets();
            SendServerAmpLists();
            SendServerPlayerInnate();
            // TODO: Cast Innate Spells
            CastClassPassives();
            SendUnlockedInlaidEldanAugmentations();
            GetActionSet(ActiveActionSet).CastAmps();

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

        private void SendServerAbilityList()
        {
            var serverAbilities = new ServerAbilities();
            foreach ((uint spell4BaseId, ICharacterSpell spell) in spells)
            {
                ISpellBaseInfo spellBaseInfo = LegacyServiceProvider.Provider.GetService<ISpellInfoManager>().GetSpellBaseInfo(spell4BaseId);
                if (spellBaseInfo == null)
                    continue;

                for (byte i = 0; i < ActionSet.MaxActionSets; i++)
                {
                    IActionSetShortcut shortcut = actionSets[i].GetShortcut(ShortcutType.SpellbookItem, spell4BaseId);
                    serverAbilities.Spells.Add(new ServerAbilities.Spell
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

            player.Session.EnqueueMessageEncrypted(serverAbilities);
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
            player.Session.EnqueueMessageEncrypted(new ServerStanceChanged
            {
                InnateIndex = InnateIndex
            });
        }
    }
}

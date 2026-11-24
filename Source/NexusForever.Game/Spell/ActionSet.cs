using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Abilities;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model.Abilities;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Network.World.Message.Static;
using NLog;

namespace NexusForever.Game.Spell
{
    public class ActionSet : IActionSet
    {
        [Flags]
        public enum ActionSetSaveMask
        {
            None             = 0x0000,
            ActionSetActions = 0x0001,
            ActionSetAmps    = 0x0002
        }

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public const byte MaxTierPoints  = 42;
        public const byte MaxActionSets  = 4;
        public const byte MaxActionCount = 48;
        public const byte MaxTier        = 9;

        /// <summary>
        /// <see cref="GameFormulaEntry"/> id holding the AMP point budget constants.
        /// </summary>
        /// <remarks>
        /// Row 942 carries dataint0 = 10 (the level below which no AMP point is granted),
        /// dataint01 = 7 (the flat base) and datafloat0 = 1.0 (points per level above the floor).
        /// The client falls back to immediates compiled into its own code when the row is missing,
        /// and two of those disagree with the shipped row (flat base 4, tier weight 0.5), and the
        /// type-225 prerequisite handler falls back to a floor of 6 against the shipped 10; so the
        /// row is the authority and the fallbacks below only mirror the shipped values.
        /// </remarks>
        public const uint AmpPointGameFormulaId = 942u;

        private const uint  fallbackAmpPointLevelFloor = 10u;
        private const uint  fallbackAmpPointFlatBase   = 7u;
        private const float fallbackAmpPointPerLevel   = 1.0f;

        /// <summary>
        /// The level below which no AMP point is granted.
        /// </summary>
        /// <remarks>
        /// The point budget and the <c>LevelGrantAmpPoints</c> prerequisite check both read the
        /// floor here, so there is one fallback and not two.
        /// </remarks>
        public static uint AmpPointLevelFloor =>
            GameTableManager.Instance.GameFormula.GetEntry(AmpPointGameFormulaId)?.Dataint0
                ?? fallbackAmpPointLevelFloor;

        public ulong Owner { get; }
        public byte Index { get; }
        public byte TierPoints { get; private set; }

        /// <summary>
        /// Total AMP points the <see cref="IActionSet"/> owner may spend.
        /// </summary>
        /// <remarks>
        /// The client recomputes this from the player's level, the item granted bonus and the
        /// Primal Matrix contribution every time any of the three changes, and stores no maximum of
        /// its own; so neither do we. 0 below level 10, 7 at level 10, 47 at level 50.
        /// </remarks>
        public byte AmpPointLimit => CalculateAmpPointLimit(
            player.Level, spellManager.BonusAmpPower, MatrixAmpPower);

        /// <summary>
        /// AMP points the <see cref="IActionSet"/> owner has left to spend.
        /// </summary>
        public byte AmpPoints
        {
            get
            {
                int remaining = AmpPointLimit - spentAmpPoints;
                return remaining <= 0 ? (byte)0 : (byte)remaining;
            }
        }

        /// <summary>
        /// AMP points granted by the Primal Matrix.
        /// </summary>
        /// <remarks>
        /// The client keeps this in a slot of its own, written only by the Primal Matrix update the
        /// server sends. That feature does not exist here, so the contribution is 0; on shipped data
        /// its maximum is 9.
        /// </remarks>
        public static uint MatrixAmpPower => 0u;

        /// <summary>
        /// Collection of <see cref="IActionSetShortcut"/> contained in the <see cref="IActionSet"/>.
        /// </summary>
        public IEnumerable<IActionSetShortcut> Actions => actions.Values.Where(a => !a.PendingDelete);

        /// <summary>
        /// Collection of <see cref="IActionSetAmp"/> contained in the <see cref="IActionSet"/>.
        /// </summary>
        public IEnumerable<IActionSetAmp> Amps => amps.Values.Where(a => !a.PendingDelete);

        private readonly Dictionary<UILocation, IActionSetShortcut> actions = new();
        private readonly Dictionary<ushort, IActionSetAmp> amps = new();

        private readonly IPlayer player;

        // not player.SpellManager: the action sets are built from inside the ISpellManager
        // constructor, before the player's own reference to it has been assigned.
        private readonly ISpellManager spellManager;

        private ushort spentAmpPoints;

        private ActionSetSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="IActionSet"/> with supplied index.
        /// </summary>
        public ActionSet(byte index, IPlayer player, ISpellManager spellManager)
        {
            Owner             = player.CharacterId;
            Index             = index;
            TierPoints        = MaxTierPoints;
            this.player       = player;
            this.spellManager = spellManager;
        }

        /// <summary>
        /// Calculate the total spendable AMP points for the supplied level and bonuses.
        /// </summary>
        /// <remarks>
        /// The client's own formula, with the <see cref="AmpPointGameFormulaId"/> constants:
        /// level below the floor gives 0, otherwise (int)((level - floor) * perLevel) + matrix +
        /// bonus + flat base. The truncation is toward zero, as the client's cvttss2si is.
        /// </remarks>
        public static byte CalculateAmpPointLimit(uint level, short bonusPower, uint matrixPower)
        {
            GameFormulaEntry entry = GameTableManager.Instance.GameFormula.GetEntry(AmpPointGameFormulaId);

            uint  levelFloor = AmpPointLevelFloor;
            uint  flatBase   = entry?.Dataint01 ?? fallbackAmpPointFlatBase;
            float perLevel   = entry?.Datafloat0 ?? fallbackAmpPointPerLevel;

            if (level < levelFloor)
                return 0;

            long total = (long)((level - levelFloor) * perLevel) + matrixPower + bonusPower + flatBase;

            if (total <= 0L)
                return 0;

            return total > byte.MaxValue ? byte.MaxValue : (byte)total;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == ActionSetSaveMask.None)
                return;

            if ((saveMask & ActionSetSaveMask.ActionSetAmps) != 0)
            {
                foreach ((ushort id, IActionSetAmp amp) in amps.OrderBy(i => i.Value.PendingDelete == true).ToList())
                {
                    if (amp.PendingDelete)
                        amps.Remove(id);

                    amp.Save(context);
                }
            }

            if ((saveMask & ActionSetSaveMask.ActionSetActions) != 0)
            {
                foreach ((UILocation location, IActionSetShortcut shortcut) in actions.OrderBy(i => i.Value.PendingDelete == true).ToList())
                {
                    if (shortcut.PendingDelete)
                        actions.Remove(location);

                    shortcut.Save(context);
                }
            }

            saveMask = ActionSetSaveMask.None;
        }

        /// <summary>
        /// Return <see cref="IActionSetShortcut"/> at supplied <see cref="UILocation"/>.
        /// </summary>
        public IActionSetShortcut GetShortcut(UILocation location)
        {
            if (!actions.TryGetValue(location, out IActionSetShortcut shortcut))
                return null;

            return shortcut.PendingDelete ? null : shortcut;
        }

        /// <summary>
        /// Return <see cref="IActionSetShortcut"/> with supplied <see cref="ShortcutType"/> and object id.
        /// </summary>
        public IActionSetShortcut GetShortcut(ShortcutType type, uint objectId)
        {
            IActionSetShortcut shortcut = actions.Values
                .Where(a => !a.PendingDelete)
                .SingleOrDefault(a => a.ShortcutType == type && a.ObjectId == objectId);

            return shortcut;
        }

        /// <summary>
        /// Return <see cref="IActionSetAmp"/> with supplied id.
        /// </summary>
        public IActionSetAmp GetAmp(ushort id)
        {
            if (!amps.TryGetValue(id, out IActionSetAmp amp))
                return null;

            return amp.PendingDelete ? null : amp;
        }

        /// <summary>
        /// Add shortcut to <see cref="IActionSet"/> to supplied <see cref="UILocation"/>.
        /// </summary>
        public void AddShortcut(UILocation location, ShortcutType type, uint objectId, byte tier)
        {
            if (actions.TryGetValue(location, out IActionSetShortcut shortcut) && !shortcut.PendingDelete)
                throw new InvalidOperationException($"Failed to add shortcut {type} {objectId} to {location}, location is occupied!");

            if (type == ShortcutType.SpellbookItem)
            {
                checked
                {
                    TierPoints -= CalculateTierCost(tier);
                }
            }

            if (shortcut != null)
            {
                shortcut.EnqueueDelete(false);
                shortcut.ShortcutType = type;
                shortcut.ObjectId     = objectId;
                shortcut.Tier         = tier;
            }
            else
                actions.Add(location, new ActionSetShortcut(this, location, type, objectId, tier));

            saveMask |= ActionSetSaveMask.ActionSetActions;

            log.Trace($"Added shortcut {type} {objectId} at {location} to action set {Index}.");
        }

        /// <summary>
        /// Add shortcut to <see cref="IActionSet"/> from an existing database model.
        /// </summary>
        public void AddShortcut(CharacterActionSetShortcutModel model)
        {
            var shortcut = new ActionSetShortcut(this, model);

            if (shortcut.ShortcutType == ShortcutType.SpellbookItem)
            {
                checked
                {
                    TierPoints -= CalculateTierCost(model.Tier);
                }
            }

            actions.Add(shortcut.Location, shortcut);
        }

        /// <summary>
        /// Update a <see cref="ShortcutType.SpellbookItem"/> shortcut with supplied tier.
        /// </summary>
        public void UpdateSpellShortcut(uint spell4BaseId, byte tier)
        {
            IActionSetShortcut shortcut = GetShortcut(ShortcutType.SpellbookItem, spell4BaseId);
            if (shortcut == null)
                throw new ArgumentException();

            checked
            {
                TierPoints += CalculateTierCost(shortcut.Tier);
                TierPoints -= CalculateTierCost(tier);
            }

            shortcut.Tier = tier;
            saveMask |= ActionSetSaveMask.ActionSetActions;
        }

        /// <summary>
        /// Remove shortcut from <see cref="IActionSet"/> at supplied <see cref="UILocation"/>.
        /// </summary>
        public void RemoveShortcut(UILocation location)
        {
            IActionSetShortcut shortcut = GetShortcut(location);
            if (shortcut == null)
                throw new ArgumentException($"Failed to remove shortcut from {location}, location isn't occupied!");

            if (shortcut.ShortcutType == ShortcutType.SpellbookItem)
            {
                checked
                {
                    TierPoints += CalculateTierCost(shortcut.Tier);
                }
            }

            if (shortcut.PendingCreate)
                actions.Remove(location);
            else
            {
                shortcut.EnqueueDelete(true);
                saveMask |= ActionSetSaveMask.ActionSetActions;
            }

            log.Trace($"Removed shortcut {shortcut.ShortcutType} {shortcut.ObjectId} at {location} from action set {Index}.");
        }

        private byte CalculateTierCost(byte tier)
        {
            if (tier > MaxTier)
                throw new ArgumentOutOfRangeException();
                
            byte cost = 0;
            for (byte i = 2; i <= tier; i++)
                cost += (byte)(i == 5 || i == 9 ? 5 : 1);

            return cost;
        }

        /// <summary>
        /// Add AMP to <see cref="IActionSet"/> with supplied id.
        /// </summary>
        public void AddAmp(ushort id)
        {
            EldanAugmentationEntry entry = GameTableManager.Instance.EldanAugmentation.GetEntry(id);
            if (entry == null)
                throw new ArgumentException($"Invalid eldan augmentation id {id}!");

            if (amps.TryGetValue(id, out IActionSetAmp amp) && !amp.PendingDelete)
                throw new InvalidOperationException($"Failed to add AMP {id}, location is already occupied!");

            if (entry.PowerCost > AmpPoints)
                throw new InvalidOperationException($"Failed to add AMP {id}, {entry.PowerCost} power exceeds the {AmpPoints} remaining of a {AmpPointLimit} point budget!");

            checked
            {
                spentAmpPoints += (ushort)entry.PowerCost;
            }

            if (amp != null)
                amp.EnqueueDelete(false);
            else
                amps.Add(id, new ActionSetAmp(this, entry, true));

            saveMask |= ActionSetSaveMask.ActionSetAmps;

            CastAmpSpell(entry);

            log.Trace($"Added AMP {id} to action set {Index}.");
        }

        /// <summary>
        /// Add AMP to <see cref="IActionSet"/> from an existing database model.
        /// </summary>
        public void AddAmp(CharacterActionSetAmpModel model)
        {
            EldanAugmentationEntry entry = GameTableManager.Instance.EldanAugmentation.GetEntry(model.AmpId);
            if (entry == null)
                throw new ArgumentException();

            // the login path does not refuse a stored selection that no longer fits the budget - a
            // character whose bonus power was reduced would otherwise fail to load. AmpPoints floors
            // at 0 until the owner respecs.
            checked
            {
                spentAmpPoints += (ushort)entry.PowerCost;
            }

            if (spentAmpPoints > AmpPointLimit)
            {
                log.Warn($"Action set {Index} of character {Owner} has {spentAmpPoints} AMP power " +
                    $"spent against a {AmpPointLimit} point budget.");
            }

            amps.Add(model.AmpId, new ActionSetAmp(this, entry, false));
        }

        /// <summary>
        /// Remove one or more AMP's from <see cref="IActionSet"/> depending on supplied <see cref="AmpRespecType"/>.
        /// </summary>
        public void RemoveAmp(AmpRespecType type, uint value)
        {
            switch (type)
            {
                case AmpRespecType.Full:
                {
                    foreach (IActionSetAmp amp in Amps.ToList())
                        RemoveAmp(amp);
                    break;
                }
                case AmpRespecType.Section:
                {
                    foreach (IActionSetAmp amp in Amps
                        .ToList()
                        .Where(a => a.Entry.EldanAugmentationCategoryId == value))
                        RemoveAmp(amp);
                    break;
                }
                case AmpRespecType.Single:
                {
                    IActionSetAmp amp = GetAmp((ushort)value);
                    RemoveAmp(amp);
                    break;
                }
                default:
                    throw new ArgumentException();
            }
        }

        private void RemoveAmp(IActionSetAmp amp)
        {
            if (amp == null)
                throw new ArgumentNullException();

            checked
            {
                spentAmpPoints -= (ushort)Math.Min(spentAmpPoints, amp.Entry.PowerCost);
            }

            if (amp.PendingCreate)
                amps.Remove((ushort)amp.Entry.Id);
            else
            {
                amp.EnqueueDelete(true);
                saveMask |= ActionSetSaveMask.ActionSetAmps;
            }

            FinishAmpSpell(amp.Entry);

            log.Trace($"Removed AMP {amp.Entry.Id} from action set {Index}.");
        }

        /// <summary>
        /// Cast the spell of every <see cref="IActionSetAmp"/> in the <see cref="IActionSet"/>.
        /// </summary>
        public void CastAmps()
        {
            foreach (IActionSetAmp amp in Amps)
                CastAmpSpell(amp.Entry);
        }

        /// <summary>
        /// Finish the spell of every <see cref="IActionSetAmp"/> in the <see cref="IActionSet"/>.
        /// </summary>
        public void FinishAmps()
        {
            foreach (IActionSetAmp amp in Amps)
                FinishAmpSpell(amp.Entry);
        }

        /// <summary>
        /// Cast the <see cref="EldanAugmentationEntry"/> spell, in the shape of a class passive.
        /// </summary>
        private void CastAmpSpell(EldanAugmentationEntry entry)
        {
            if (entry.Spell4IdAugment == 0u)
                return;

            if (player.HasSpell(entry.Spell4IdAugment, out ISpell _))
                return;

            player.CastSpell(entry.Spell4IdAugment, new SpellParameters
            {
                UserInitiatedSpellCast = false
            });
        }

        /// <summary>
        /// Finish the <see cref="EldanAugmentationEntry"/> spell, if it is running.
        /// </summary>
        private void FinishAmpSpell(EldanAugmentationEntry entry)
        {
            if (entry.Spell4IdAugment == 0u)
                return;

            if (player.HasSpell(entry.Spell4IdAugment, out ISpell spell))
                spell.Finish();
        }

        /// <summary>
        /// Build a network representation of the <see cref="IActionSetShortcut"/>'s in the <see cref="IActionSet"/>.
        /// </summary>
        public ServerActionSet BuildServerActionSet()
        {
            var serverActionSet = new ServerActionSet
            {
                SpecIndex   = Index,
                Unlocked    = 1,
                Result      = LimitedActionSetResult.Ok
            };

            for (UILocation i = 0; i < (UILocation)MaxActionCount; i++)
            {
                IActionSetShortcut action = GetShortcut(i);
                serverActionSet.Actions.Add(new ServerActionSet.Action
                {
                    ShortcutType = action?.ShortcutType ?? ShortcutType.None,
                    ObjectId     = action?.ObjectId ?? 0,
                    Location     = new ItemLocation
                    {
                        // TODO: this might not be correct, what about shortcuts that aren't spells?
                        Location = action != null ? InventoryLocation.Ability : (InventoryLocation)300, // no idea why 300, this is what retail did
                        BagIndex = (uint)(action?.Location ?? i)
                    }
                });
            }

            return serverActionSet;
        }

        /// <summary>
        /// Build a network representation of the <see cref="IActionSetAmp"/>'s in the <see cref="IActionSet"/>.
        /// </summary>
        public ServerAmpList BuildServerAmpList()
        {
            var serverAmpList = new ServerAmpList
            {
                SpecIndex = Index
            };

            foreach (IActionSetAmp amp in Amps)
                serverAmpList.Amps.Add((ushort)amp.Entry.Id);

            return serverAmpList;
        }
    }
}

using System.Collections.Immutable;
using NexusForever.Game.Abstract.Customisation;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.GameTable.Static;
using NexusForever.Shared;
using NLog;

namespace NexusForever.Game.Customisation
{
    public class CustomisationManager : Singleton<ICustomisationManager>, ICustomisationManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private ImmutableDictionary<uint, ICustomisationLabel> customisationLabels;
        private ImmutableDictionary<(uint Label, uint Value), ICustomisationSelection> customisationSelections;

        private ImmutableDictionary<(Race, Sex), ICustomisationInfoCollection> customisationInfoCollections;
        private ImmutableDictionary<(Race, Faction), ICustomisationInfo> raceCustomisationInfos;

        /// <summary>
        /// Initialise <see cref="ICustomisationManager"/> and any associated resources.
        /// </summary>
        public void Initialise()
        {
            log.Info("Initialising customisation data...");

            InitialiseLabels();
            InitialiseSelections();

            InitialiseCustomisations();
            InitialiseRaceCustomisations();
        }

        private void InitialiseLabels()
        {
            customisationLabels = GameTableManager.Instance.CharacterCustomizationLabel.Entries
                .Select(e => (ICustomisationLabel)new CustomisationLabel(e))
                .ToImmutableDictionary(l => l.Id, l => l);

            log.Trace($"Initialised {customisationLabels.Count} customisation label(s).");
        }

        private void InitialiseSelections()
        {
            customisationSelections = GameTableManager.Instance.CharacterCustomizationSelection.Entries
                // client also skips entry 1
                .Where(e => e.Id != 1)
                .Select(e => (ICustomisationSelection)new CustomisationSelection(e))
                .ToImmutableDictionary(s => (s.Label, s.Value), s => s);

            log.Trace($"Initialised {customisationSelections.Count} customisation selection(s).");
        }

        private void InitialiseCustomisations()
        {
            // initialise customisation entries that belong to a sex only
            ILookup<Sex, ICustomisationInfo> sexCustomisations = GameTableManager.Instance.CharacterCustomization.Entries
                .Where(e => (Race)e.RaceId == Race.None)
                .Select(InitialiseCustomisation)
                .Where(c => c != null)
                .ToLookup(c => c.Sex);

            // initialise customistion entries that belong to a race and sex
            customisationInfoCollections = GameTableManager.Instance.CharacterCustomization.Entries
                .Where(e => (Race)e.RaceId != Race.None)
                .Select(InitialiseCustomisation)
                .Where(c => c != null)
                .GroupBy(c => new { c.Race, c.Sex })
                .ToImmutableDictionary(g => (g.Key.Race, g.Key.Sex),g => (ICustomisationInfoCollection)new CustomisationInfoCollection(g.Key.Race, g.Key.Sex, g.ToList(), sexCustomisations[g.Key.Sex]));

            log.Trace($"Initialised {customisationInfoCollections.Count} race, sex customisation(s).");
        }

        private void InitialiseRaceCustomisations()
        {
            raceCustomisationInfos = GameTableManager.Instance.CharacterCustomization.Entries
                // race customisations are handled separately and are not provided by the client
                .Where(e => (((CharacterCustomisationEntryFlags)e.Flags) & CharacterCustomisationEntryFlags.Race) != 0)
                .Select(InitialiseCustomisation)
                .ToImmutableDictionary(c => (c.Race, customisationLabels[c.Label[0]].Faction), c => c);

            log.Trace($"Initialised {raceCustomisationInfos.Count} race, faction customisation(s).");
        }

        private ICustomisationInfo InitialiseCustomisation(CharacterCustomizationEntry entry)
        {
            CharacterCustomisationEntryFlags flags = (CharacterCustomisationEntryFlags)entry.Flags;
            // client does this... not sure why we need both disabled and enabled flags
            if ((flags & CharacterCustomisationEntryFlags.Disabled) != 0 || (flags & CharacterCustomisationEntryFlags.Enabled) == 0)
                return null;
            
            if (entry.CharacterCustomizationLabelId[0] == 0)
                // this is a special case, some customisation entries only have the second label and value set
                // the client registers these with the first label and value
                return new CustomisationInfo((Race)entry.RaceId, (Sex)entry.Gender, new[] { entry.CharacterCustomizationLabelId[1], 0u }, new[] { entry.Value[1], 0u }, (ItemSlot)entry.ItemSlotId, entry.ItemDisplayId);

            return new CustomisationInfo(entry);
        }

        /// <summary>
        /// Validate customisation data is valid.
        /// </summary>
        public bool Validate(Race race, Sex sex, Faction faction, IList<(uint Label, uint Value)> customisations)
        {
            if (!raceCustomisationInfos.ContainsKey((race, faction)))
                return false;

            if (!customisationInfoCollections.ContainsKey((race, sex)))
                return false;

            foreach ((uint Label, uint Value) customisation in customisations)
            {
                if (!customisationSelections.ContainsKey(customisation))
                    return false;

                if (customisationLabels.TryGetValue(customisation.Label, out ICustomisationLabel customisationLabel)
                    && customisationLabel.Faction != 0
                    && customisationLabel.Faction != faction)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Calculate the price of customisation in credits.
        /// </summary>
        public uint CalculateCostCredits(IPlayer player, Race race, Sex sex, IList<(uint Label, uint Value)> customisations, IEnumerable<float> bones)
        {
            if (!customisationInfoCollections.TryGetValue((race, sex), out ICustomisationInfoCollection customisationCollection))
                throw new ArgumentOutOfRangeException();

            // determine which customisations have changed
            List<(uint Label, uint Value)> changedCustomisations = customisations
                .Except(player.AppearanceManager
                    .GetCustomisations().Select(c => (c.Label, c.Value)))
                .ToList();

            uint cost = 0;
            foreach ((uint Label, uint Value) customisation in changedCustomisations)
                if (customisationSelections.TryGetValue(customisation, out ICustomisationSelection custimisationSelection))
                    cost += custimisationSelection.Cost;

            // only add the price for bones if the face hasn't changed
            if (!bones.SequenceEqual(player.AppearanceManager
                    .GetBones().Select(b => b.BoneValue))
                // client has a hardcoded array of labels which are face changes
                && !changedCustomisations.Any(c => c.Label is 1u or 21u or 22u))
            {
                GameFormulaEntry entry = GameTableManager.Instance.GameFormula.GetEntry(1175);
                cost += entry?.Dataint0 ?? 0u;
            }

            return cost;
        }

        /// <summary>
        /// Calculate the price of customisation in tokens.
        /// </summary>
        public uint CalculateCostTokens(IPlayer player, Race race, Sex sex)
        {
            if (player.Race != race || player.Sex != sex)
            {
                if (!raceCustomisationInfos.TryGetValue((race, player.Faction1), out ICustomisationInfo customisationInfo))
                    throw new ArgumentOutOfRangeException();

                return customisationSelections.TryGetValue((customisationInfo.Label[0], customisationInfo.Value[0]), out ICustomisationSelection selection) ? selection.Cost : 0u;
            }
            else
            {
                GameFormulaEntry entry = GameTableManager.Instance.GameFormula.GetEntry(1176);
                return entry?.Dataint0 ?? 10u;
            }
        }

        /// <summary>
        /// Return a collection of <see cref="IItemVisual"/> for customisation.
        /// </summary>
        public IEnumerable<IItemVisual> GetItemVisuals(Race race, Sex sex, IList<(uint Label, uint Value)> customisations)
        {
            if (!customisationInfoCollections.TryGetValue((race, sex), out ICustomisationInfoCollection customisationCollection))
                throw new ArgumentOutOfRangeException();

            foreach ((uint label, uint value) in customisations)
            {
                foreach (ICustomisationInfo customisationInfo in customisationCollection.GetCustomisations(label, value, customisations))
                {
                    yield return new ItemVisual
                    {
                        Slot      = customisationInfo.Slot,
                        DisplayId = (ushort)customisationInfo.DisplayId
                    };
                }
            }
        }
    }
}

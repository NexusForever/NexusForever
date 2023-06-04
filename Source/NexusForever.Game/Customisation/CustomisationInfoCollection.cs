using System.Collections.Immutable;
using NexusForever.Game.Abstract.Customisation;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Customisation
{
    public class CustomisationInfoCollection : ICustomisationInfoCollection
    {
        public Race Race { get; }
        public Sex Sex { get; }

        private readonly ImmutableDictionary<(uint Label, uint Value), ImmutableList<ICustomisationInfo>> primaryCustomisations;

        /// <summary>
        /// Create new <see cref="ICustomisationInfoCollection"/>.
        /// </summary>
        public CustomisationInfoCollection(Race race, Sex sex, IEnumerable<ICustomisationInfo> customisationInfos, IEnumerable<ICustomisationInfo> sexCustomisationInfos)
        {
            Race = race;
            Sex  = sex;

            // concatenate sex customisation entries where no race customisation exists for label and value
            // these entries are used as a fall back if no race label and value is defined
            primaryCustomisations = customisationInfos
                .Concat(sexCustomisationInfos
                    .Where(a => !customisationInfos
                        .Any(a2 => a2.Label.SequenceEqual(a.Label) && a2.Value.SequenceEqual(a.Value))))
                .GroupBy(c => new { Label = c.Label[0], Value = c.Value[0] })
                .ToImmutableDictionary(g => (g.Key.Label, g.Key.Value), g => g.ToImmutableList());
        }

        /// <summary>
        /// Return collection of primary and secondary <see cref="ICustomisationInfo"/> for supplied primary and secondary labels.
        /// </summary>
        public IEnumerable<ICustomisationInfo> GetCustomisations(uint label, uint value, IList<(uint Label, uint Value)> customisations)
        {
            var slots = new HashSet<ItemSlot>();
            foreach (ICustomisationInfo customisation in GetSecondaryCustomisations(label, value, customisations)
                .Append(GetPrimaryCustomisation(label, value)))
            {
                if (customisation == null)
                    continue;

                if (slots.Add(customisation.Slot))
                    yield return customisation;
            }
        }

        private ICustomisationInfo GetPrimaryCustomisation(uint label, uint value)
        {
            if (!primaryCustomisations.TryGetValue((label, value), out ImmutableList<ICustomisationInfo> customisationInfos))
                return null;

            // find customisation that only has primary label and value
            return customisationInfos.SingleOrDefault(e => e.Label[1] == 0 && e.Value[1] == 0);
        }

        private IEnumerable<ICustomisationInfo> GetSecondaryCustomisations(uint label, uint value, IList<(uint Label, uint Value)> customisations)
        {
            if (!primaryCustomisations.TryGetValue((label, value), out ImmutableList<ICustomisationInfo> customisationInfos))
                return Enumerable.Empty<ICustomisationInfo>();

            // find customisations with secondary label and values
            return customisationInfos.Where(e => e.Label[1] != 0 && customisations.Contains((e.Label[1], e.Value[1]))) ?? Enumerable.Empty<ICustomisationInfo>();
        }
    }
}

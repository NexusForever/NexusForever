using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;

namespace NexusForever.Game.Abstract.Customisation
{
    public interface ICustomisationManager
    {
        /// <summary>
        /// Initialise <see cref="ICustomisationManager"/> and any associated resources.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Validate customisation data is valid.
        /// </summary>
        bool Validate(Race race, Sex sex, Faction faction, IList<(uint Label, uint Value)> customisations);

        /// <summary>
        /// Calculate the price of customisation in credits.
        /// </summary>
        uint CalculateCostCredits(IPlayer player, Race race, Sex sex, IList<(uint Label, uint Value)> customisations, IEnumerable<float> bones);

        /// <summary>
        /// Calculate the price of customisation in tokens.
        /// </summary>
        uint CalculateCostTokens(IPlayer player, Race race, Sex sex);

        /// <summary>
        /// Return a collection of <see cref="IItemVisual"/> for customisation.
        /// </summary>
        IEnumerable<IItemVisual> GetItemVisuals(Race race, Sex sex, IList<(uint Label, uint Value)> customisations);
    }
}
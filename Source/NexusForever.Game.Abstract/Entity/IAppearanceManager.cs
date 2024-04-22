using NexusForever.Database.Character;
using NexusForever.Game.Abstract.Customisation;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IAppearanceManager : IDatabaseCharacter
    {
        /// <summary>
        /// Return a collection of <see cref="ICustomisation"/> for <see cref="IPlayer"/>.
        /// </summary>
        IEnumerable<ICustomisation> GetCustomisations();

        /// <summary>
        /// Return a collection of <see cref="IAppearance"/> for <see cref="IPlayer"/>.
        /// </summary>
        IEnumerable<IAppearance> GetAppearances();

        /// <summary>
        /// Return a collection of <see cref="IBone"/> for <see cref="IPlayer"/>.
        /// </summary>
        IEnumerable<IBone> GetBones();

        /// <summary>
        /// Update <see cref="IPlayer"/> appearance.
        /// This will update, <see cref="Race"/>, <see cref="Sex"/>, customisations and bones.
        /// </summary>
        /// <remarks>
        /// This will do no validation, for customisation validation see <see cref="ICustomisationManager.Validate(Race, Sex, Faction, IList{ValueTuple{uint, uint}})"/>.
        /// </remarks>
        void Update(Race race, Sex sex, IList<(uint Label, uint Value)> customisations, IList<float> bones);
    }
}

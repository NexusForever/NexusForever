using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Customisation
{
    public interface ICustomisationInfoCollection
    {
        Race Race { get; }
        Sex Sex { get; }

        /// <summary>
        /// Return collection of primary and secondary <see cref="ICustomisationInfo"/> for supplied primary and secondary labels.
        /// </summary>
        IEnumerable<ICustomisationInfo> GetCustomisations(uint label, uint value, IList<(uint Label, uint Value)> customisations);
    }
}
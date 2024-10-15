using NexusForever.GameTable.Model;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Map
{
    public interface IMap : IUpdate
    {
        WorldEntry Entry { get; }

        /// <summary>
        /// Initialise <see cref="IMap"/> with <see cref="WorldEntry"/>.
        /// </summary>
        void Initialise(WorldEntry entry);

        /// <summary>
        /// Return a string containing debug information about the map.
        /// </summary>
        string WriteDebugInformation();
    }
}

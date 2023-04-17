using System;

namespace NexusForever.Shared
{
    public interface IWorldManager
    {
        /// <summary>
        /// Initialise <see cref="IWorldManager"/> and any related resources.
        /// </summary>
        void Initialise(Action<double> updateAction);

        /// <summary>
        /// Request shutdown of <see cref="IWorldManager"/> and any related resources.
        /// </summary>
        void Shutdown();
    }
}
using NexusForever.Game.Static;

namespace NexusForever.Game.Abstract
{
    public interface IDisableManager
    {
        void Initialise();

        /// <summary>
        /// Returns if <see cref="DisableType"/> and objectId are disabled.
        /// </summary>
        bool IsDisabled(DisableType type, uint objectId);

        /// <summary>
        /// Return the disable reason for <see cref="DisableType"/> and objectId.
        /// </summary>
        string GetDisableNote(DisableType type, uint objectId);
    }
}
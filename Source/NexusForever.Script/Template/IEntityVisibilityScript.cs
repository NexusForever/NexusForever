using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Script.Template
{
    public interface IEntityVisibilityScript : IScript
    {
        /// <summary>
        /// Return whether this entity can be shown to the supplied player.
        /// </summary>
        bool CanBeSeenBy(IPlayer viewer);
    }
}

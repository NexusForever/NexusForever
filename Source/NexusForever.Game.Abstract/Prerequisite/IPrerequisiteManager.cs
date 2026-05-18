using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Abstract.Prerequisite
{
    public interface IPrerequisiteManager
    {
        /// <summary>
        /// Checks if <see cref="IPlayer"/> meets supplied prerequisite.
        /// </summary>
        bool Meets(IPlayer player, uint prerequisiteId);

        /// <summary>
        /// Checks if <see cref="IPlayer"/> meets supplied prerequisite.
        /// </summary>
        bool Meets(IPlayer player, uint prerequisiteId, IPrerequisiteParameters parameters);
    }
}

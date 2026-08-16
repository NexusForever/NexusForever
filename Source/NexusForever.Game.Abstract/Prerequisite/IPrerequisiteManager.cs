using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Abstract.Prerequisite
{
    public interface IPrerequisiteManager
    {
        /// <summary>
        /// Checks if <see cref="IUnitEntity"/> meets supplied prerequisite.
        /// </summary>
        bool Meets(IUnitEntity subject, uint prerequisiteId);

        /// <summary>
        /// Checks if <see cref="IUnitEntity"/> meets supplied prerequisite with a secondary unit context.
        /// </summary>
        bool Meets(IUnitEntity subject, uint prerequisiteId, IUnitEntity secondaryUnit);

        /// <summary>
        /// Checks if <see cref="IUnitEntity"/> meets supplied prerequisite.
        /// </summary>
        bool Meets(IUnitEntity subject, uint prerequisiteId, IPrerequisiteParameters parameters);
    }
}

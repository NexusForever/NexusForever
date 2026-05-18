using System.Collections.Immutable;
using NexusForever.Database;
using NexusForever.Database.World;
using NexusForever.Database.World.Model;

namespace NexusForever.Script.Template.Filter.Dynamic
{
    public class ScriptFilterDynamicEntitySpline : IScriptFilterDynamicEntitySpline
    {
        #region Dependency Injection

        private readonly IDatabaseManager databaseManager;

        public ScriptFilterDynamicEntitySpline(
            IDatabaseManager databaseManager)
        {
            this.databaseManager = databaseManager;
        }

        #endregion

        /// <summary>
        /// Modify <see cref="IScriptFilterParameters"/> dynamically at runtime.
        /// </summary>
        public void Filter(IScriptFilterParameters parameters)
        {
            ImmutableList<EntityModel> entities = databaseManager.GetDatabase<WorldDatabase>().GetEntitiesWithSpline();
            if (entities.Count == 0)
                return;

            parameters.CreatureId ??= new HashSet<uint>();
            foreach (EntityModel model in entities)
                parameters.CreatureId.Add(model.Creature);
        }
    }
}

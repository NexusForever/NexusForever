using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Trigger;
using NexusForever.Game.Abstract.Map.Search;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Script;
using NexusForever.Script.Template;
using NexusForever.Shared.Game;

namespace NexusForever.Game.Entity.Trigger
{
    public class WorldLocationTriggerEntity : GridEntity, IWorldLocationTriggerEntity
    {
        private WorldLocation2Entry entry;

        private readonly Dictionary<uint, IGridEntity> triggerEntities = [];
        private readonly UpdateTimer timer = new(TimeSpan.FromMilliseconds(100));

        #region Dependency Injection

        private readonly IScriptManager scriptManager;
        private readonly ISearchCheckFactory searchCheckFactory;
        private readonly IGameTableManager gameTableManager;

        public WorldLocationTriggerEntity(
            IScriptManager scriptManager,
            ISearchCheckFactory searchCheckFactory,
            IGameTableManager gameTableManager)
        {
            this.scriptManager = scriptManager;
            this.searchCheckFactory = searchCheckFactory;
            this.gameTableManager = gameTableManager;
        }

        #endregion

        /// <summary>
        /// Initialise trigger with supplied world location id.
        /// </summary>
        public void Initialise(uint id)
        {
            if (entry != null)
                throw new InvalidOperationException("TriggerEntity is already initialised!");

            entry            = gameTableManager.WorldLocation2.GetEntry(id);
            scriptCollection = scriptManager.InitialiseOwnedScripts<IWorldLocationTriggerEntity>(this, entry.Id);
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public override void Update(double lastTick)
        {
            base.Update(lastTick);

            timer.Update(lastTick);
            if (timer.HasElapsed)
            {
                CheckTriggerEntities();
                timer.Reset();
            }
        }
        private void CheckTriggerEntities()
        {
            var position = new Vector3(entry.Position0, entry.Position1, entry.Position2);

            ISearchCheckRange<IGridEntity> check = searchCheckFactory.CreateCheck<IGridEntity, ISearchCheckRange<IGridEntity>>();
            check.Initialise(position, entry.Radius);
            List<IGridEntity> searchEntities = Map.Search(position, entry.Radius, check).ToList();

            foreach (IGridEntity entity in searchEntities.Except(triggerEntities.Values))
            {
                triggerEntities.Add(entity.Guid, entity);
                InvokeScriptCollection<IWorldLocationTriggerScript>(s => s.OnEntityEnter(entity));
            }

            foreach (IGridEntity entity in triggerEntities.Values.Except(searchEntities))
            {
                InvokeScriptCollection<IWorldLocationTriggerScript>(s => s.OnEntityLeave(entity));
                triggerEntities.Remove(entity.Guid);
            }
        }
    }
}

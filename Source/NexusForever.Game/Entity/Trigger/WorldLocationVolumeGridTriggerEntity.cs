using NexusForever.Game.Abstract.Entity.Trigger;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Script;

namespace NexusForever.Game.Entity.Trigger
{
    public class WorldLocationVolumeGridTriggerEntity : VolumeGridTriggerEntity, IWorldLocationVolumeGridTriggerEntity
    {
        public WorldLocation2Entry Entry { get; private set; }

        #region Dependency Injection

        private readonly IGameTableManager gameTableManager;

        public WorldLocationVolumeGridTriggerEntity(
            IScriptManager scriptManager,
            IGameTableManager gameTableManager)
            : base(scriptManager)
        {
            this.gameTableManager = gameTableManager;
        }

        #endregion

        /// <summary>
        /// Initialise trigger with supplied world location id and objective object id.
        /// </summary>
        public void Initialise(uint worldLocationId, uint objectId)
        {
            if (Entry != null)
                throw new InvalidOperationException("TriggerEntity is already initialised!");

            Entry = gameTableManager.WorldLocation2.GetEntry(worldLocationId);

            Initialise(worldLocationId, Entry.Radius, objectId);
        }
    }
}

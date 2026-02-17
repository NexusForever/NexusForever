using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using System.Numerics;


namespace NexusForever.Game.Prerequisite.Check
{
    public class PrerequisiteCheckDistanceToWorldLocation : BasePrerequisiteHandler, IPrerequisiteCheck
    {
        #region Dependency Injection

        IGameTableManager gameTableManager;
        public PrerequisiteCheckDistanceToWorldLocation(ILogger<BasePrerequisiteHandler> log, IGameTableManager gameTableManager) : base(log)
        {
            this.gameTableManager = gameTableManager;
        }
        #endregion

        /// TODO: The weird (int) cast to float in valuef stems from possibly maxInt values being used in the gametables. 
        // Investigate further and possibly change value to be int across the entire chain. For now, an int cast is used to wrap around the maxInt to negative values.
        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            WorldLocation2Entry worldLocation = gameTableManager.WorldLocation2.GetEntry(objectId);

            if (worldLocation == null)
            {
                log.LogError($"WorldLocation2Entry with ID {objectId} not found in game table.");
                return false;
            }
            Vector3 worldPosition = new(worldLocation.Position0, worldLocation.Position1, worldLocation.Position2);
            float distance = player.GetDistanceTo(worldPosition);
            float valuef =  (int) value; // Mirrors client behavior
            return MatchCompareable(distance, valuef, comparison, PrerequisiteType.DistanceToWorldLocation);
        }
    }
}

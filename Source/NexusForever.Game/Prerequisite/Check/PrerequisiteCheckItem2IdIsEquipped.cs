using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.Item2IdIsEquipped)]
    public class PrerequisiteCheckItem2IdIsEquipped: BasePrerequisiteHandler, IPrerequisiteCheck
    {
        #region Dependency Injection

        public PrerequisiteCheckItem2IdIsEquipped(
            ILogger<BasePrerequisiteHandler> log)
            : base(log)
        {
        }

        #endregion
        // TODO: There is Prerequisite.ID = 8002 with ObjectId 8001 while objectId is unusued. Investigate further if it introduces issues.
        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            if (objectId != 0)
                log.LogWarning($"Unused ObjectId {objectId} in PrerequisiteType {PrerequisiteType.Item2IdIsEquipped}.");

            bool hasItem = player.Inventory.HasItem(value, InventoryLocation.Equipped);
            return MatchBoolean(hasItem, comparison, PrerequisiteType.Item2IdIsEquipped);
        }
    }
}

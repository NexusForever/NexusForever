
using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Prerequisite;

// ObjectId is not used in this check, however gametable queries for prerequisites have shown a non zero value for objectId, if it becomes a problem investigate further
namespace NexusForever.Game.Prerequisite.Check
{
    public class PrerequisiteCheckItemOnCharacter: BasePrerequisiteHandler, IPrerequisiteCheck
    {
        #region Dependency Injection
        public PrerequisiteCheckItemOnCharacter(
            ILogger<BasePrerequisiteHandler> log)
            : base(log)
        {
        }
        #endregion
        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            ICollection<InventoryLocation> locations =
            [
                InventoryLocation.Equipped,
                InventoryLocation.Inventory,
                InventoryLocation.Unknown8,
                InventoryLocation.Unknown9
            ];
            bool hasItem = player.Inventory.HasItem(value, locations);

            return MatchBoolean(hasItem, comparison, PrerequisiteType.ItemOnCharacter);

        }
    }
}

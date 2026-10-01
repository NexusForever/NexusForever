using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.ItemProficiency)]
    public class PrerequisiteCheckItemProficiency : BasePrerequisiteHandler, IPrerequisiteCheck
    {
        #region Dependency Injection

        public PrerequisiteCheckItemProficiency(
            ILogger<BasePrerequisiteHandler> log)
            : base(log)
        {
        }

        #endregion

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            ItemProficiency playerProficiencies = player.GetItemProficiencies();
            ItemProficiency requiredProficiency = (ItemProficiency)value;
            
            bool hasProficiency = (playerProficiencies & requiredProficiency) != 0;
            return MatchBoolean(hasProficiency, comparison, PrerequisiteType.ItemProficiency);
        }
    }
}

using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.Vital)]
    public class PrerequisiteCheckVital : BasePrerequisiteHandler, IPrerequisiteCheck
    {
        #region Dependency Injection

        public PrerequisiteCheckVital(
            ILogger<BasePrerequisiteHandler> log)
            : base(log)
        {
        }

        #endregion

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            float vitalValue = player.GetVitalValue((Vital)objectId);
            return MatchCompareable(vitalValue, value, comparison, PrerequisiteType.Vital);
        }
    }
}

using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.Gender)]
    public class PrerequisiteCheckGender : BasePrerequisiteHandler, IPrerequisiteCheck
    {
        #region Dependency Injection

        public PrerequisiteCheckGender(
            ILogger<BasePrerequisiteHandler> log)
            : base(log)
        {
        }

        #endregion

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            return MatchEnum(player.Sex, (Sex)value, comparison, PrerequisiteType.Gender);
        }
    }
}

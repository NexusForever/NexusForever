using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Prerequisite;

// ObjectId is not used in this check, however gametable queries for prerequisites have shown a non zero value for objectId, if it becomes a problem investigate further
namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.Race)]
    public class PrerequisiteCheckRace : BasePrerequisiteHandler, IPrerequisiteCheck
    {
        #region Dependency Injection

        public PrerequisiteCheckRace(
            ILogger<BasePrerequisiteHandler> log)
            : base(log)
        {
        }

        #endregion

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            return MatchEnum(player.Race, (Race)value, comparison, PrerequisiteType.Race);
        }
    }
}

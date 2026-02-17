using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.MountUsage)]
    public class PrerequisiteCheckMountUsage : BasePrerequisiteHandler,IPrerequisiteCheck
    {
        public PrerequisiteCheckMountUsage(ILogger<BasePrerequisiteHandler> log) : base(log)
        {
        }

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {

            // TODO: Only used in Mount check prerequisites. Its use is unknown.

            return true;
        }
    }
}

using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.RapidTransport)]
    public class PrerequisiteCheckRapidTransport : IPrerequisiteCheck
    {
        public bool Meets(IUnitEntity subject, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            // TODO: Prerequisite 37806 is the only known type 269 use. It prevents recall while the client is
            // in its "Unhealthy Time" state. The corresponding server state and operand semantics are unknown.

            // Return true until implemented so unsupported prerequisites do not block spell casts.
            return true;
        }
    }
}

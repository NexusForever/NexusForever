using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Abstract.Prerequisite
{
    public interface IPrerequisiteCheck
    {
        bool Meets(IUnitEntity subject, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters);
    }
}

using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Abstract.Prerequisite
{
    public interface IPrerequisiteCheck
    {
        bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters);
    }
}

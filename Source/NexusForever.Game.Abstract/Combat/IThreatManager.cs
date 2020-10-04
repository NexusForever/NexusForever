using NexusForever.Game.Abstract.Entity;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Combat
{
    public interface IThreatManager : IUpdate, IEnumerable<IHostileEntity>
    {
        void AddThreat(IUnitEntity target, int threat);
        void ClearThreatList();
        IEnumerable<IHostileEntity> GetThreatList();
        void RemoveTarget(uint unitId);
    }
}
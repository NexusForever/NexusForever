using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Abstract.Combat
{
    public interface IHostileEntity
    {
        uint HatedUnitId { get; }
        IUnitEntity Owner { get; }
        uint Threat { get; }

        void AdjustThreat(int threatDelta);
        IUnitEntity GetEntity(IWorldEntity requester);
    }
}
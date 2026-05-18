using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IVehiclePassenger
    {
        VehicleSeatType SeatType { get; }
        byte SeatPosition { get; }
        uint Guid { get; }
    }
}
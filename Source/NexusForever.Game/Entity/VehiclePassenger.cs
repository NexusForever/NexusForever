using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Entity
{
    public class VehiclePassenger : IVehiclePassenger
    {
        public VehicleSeatType SeatType { get; }
        public byte SeatPosition { get; }
        public uint Guid { get; }

        public VehiclePassenger(VehicleSeatType seatType, byte seatPosition, uint guid)
        {
            SeatType     = seatType;
            SeatPosition = seatPosition;
            Guid         = guid;
        }
    }
}

using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Entity
{
    public class VehiclePassenger
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

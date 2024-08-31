using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerVehiclePassengerSet)]
    public class ServerVehiclePassengerSet : IWritable
    {
        public uint Self { get; set; }
        public uint Vehicle { get; set; }
        public VehicleSeatType SeatType { get; set; }
        public byte SeatPosition { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Self);
            writer.Write(Vehicle);
            writer.Write(SeatType, 2u);
            writer.Write(SeatPosition, 3u);
        }
    }
}

using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Entity
{
    [Message(GameMessageOpcode.ServerMount)]
    public class ServerMount : IWritable
    {
        public uint PassengerUnitId { get; set; }
        public uint MountUnitId { get; set; }
        public VehicleSeatType SeatType { get; set; }
        public byte SeatPosition { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PassengerUnitId);
            writer.Write(MountUnitId);
            writer.Write(SeatType, 2u);
            writer.Write(SeatPosition, 3u);
        }
    }
}

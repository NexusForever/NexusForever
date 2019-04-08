using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerVehiclePassengerAdd)]
    public class ServerVehiclePassengerAdd : IWritable
    {
        public uint Self { get; set; }
        public VehicleSeatType SeatType { get; set; }
        public byte SeatPosition { get; set; }
        public uint UnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Self);
            writer.Write(SeatType, 2u);
            writer.Write(SeatPosition, 3u);
            writer.Write(UnitId);
        }
    }
}

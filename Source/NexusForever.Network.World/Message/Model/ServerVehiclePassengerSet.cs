using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    /// <summary>
    /// Should only be used during <see cref="GridEntity"/> AddVisible() method. It is used to "connect" an entity to their vehicle.
    /// </summary>
    /// <remarks>
    /// Packets indicate this was only used when an entity and their vehicle were created at the same time, e.g. Player logs in and another Player nearby is already mounted.
    /// </remarks>
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

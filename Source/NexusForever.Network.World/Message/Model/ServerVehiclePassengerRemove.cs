using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerVehiclePassengerRemove)]
    public class ServerVehiclePassengerRemove : IWritable
    {
        public uint Self { get; set; }
        public uint Passenger { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Self);
            writer.Write(Passenger);
        }
    }
}

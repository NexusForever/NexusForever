using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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

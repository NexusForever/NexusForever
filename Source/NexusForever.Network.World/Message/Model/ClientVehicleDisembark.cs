using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientVehicleDisembark)]
    public class ClientVehicleDisembark : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}

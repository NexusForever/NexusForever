using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientVehicleDisembark)]
    public class ClientVehicleDisembark : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}

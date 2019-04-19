using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientPlayerMovementSpeedUpdate)]
    public class ClientPlayerMovementSpeedUpdate : IReadable 
    {
        public uint Speed { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Speed = reader.ReadUInt();
        }
    }
}

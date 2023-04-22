using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCancelEffect)]
    public class ClientCancelEffect : IReadable
    {
        public uint ServerUniqueId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ServerUniqueId  = reader.ReadUInt();
        }
    }
}

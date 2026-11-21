using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Option
{
    [Message(GameMessageOpcode.ClientRequestInputKeySet)]
    public class ClientRequestInputKeySet : IReadable
    {
        public ulong CharacterId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            CharacterId = reader.ReadULong();
        }
    }
}

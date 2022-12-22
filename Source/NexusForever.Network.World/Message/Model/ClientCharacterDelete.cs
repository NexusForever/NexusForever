using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCharacterDelete)]
    public class ClientCharacterDelete : IReadable
    {
        public ulong CharacterId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            CharacterId = reader.ReadULong();
        }
    }
}

using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pregame
{
    [Message(GameMessageOpcode.ClientCharacterRename)]
    public class ClientCharacterRename : IReadable
    {
        public ulong CharacterId { get; private set; }
        public string Name { get; private set; }

        public void Read(GamePacketReader reader)
        {
            CharacterId = reader.ReadULong();
            Name = reader.ReadWideString();
        }
    }
}

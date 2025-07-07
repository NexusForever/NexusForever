using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pregame
{
    // Sent when the lua function CharacterScreenLib::SelectCharacter is called.
    [Message(GameMessageOpcode.ClientCharacterSelect)]
    public class ClientCharacterSelect : IReadable
    {
        public ulong CharacterId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            CharacterId = reader.ReadULong();
        }
    }
}

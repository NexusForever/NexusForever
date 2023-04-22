using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCharacterCreate)]
    public class ClientCharacterCreate : IReadable
    {
        public uint CharacterCreationId { get; private set; }
        public string Name { get; private set; }
        public byte Path { get; private set; }
        public List<uint> Labels { get; } = new();
        public List<uint> Values { get; } = new();
        public List<float> Bones { get; } = new();

        public void Read(GamePacketReader reader)
        {
            CharacterCreationId = reader.ReadUInt();
            Name                = reader.ReadWideString();
            Path                = reader.ReadByte(3);

            uint customisationCount = reader.ReadUInt();
            for (uint i = 0u; i < customisationCount; i++)
                Labels.Add(reader.ReadUInt());
            for (uint i = 0u; i < customisationCount; i++)
                Values.Add(reader.ReadUInt());

            uint boneCount = reader.ReadUInt();
            for (uint i = 0u; i < boneCount; i++)
                Bones.Add(reader.ReadSingle());
        }
    }
}

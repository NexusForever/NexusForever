using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCharacterAppearanceChange)]
    public class ClientCharacterAppearanceChange : IReadable
    {
        public byte Race { get; private set; } // 5
        public byte Sex { get; private set; } // 2
        public uint CustomisationCount { get; private set; }
        public List<uint> Labels { get; private set; } = new List<uint>();
        public List<uint> Values { get; private set; } = new List<uint>();
        public uint BoneCount { get; private set; }
        public List<float> Bones { get; private set; } = new List<float>();
        public bool UseServiceTokens { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Race = reader.ReadByte(5);
            Sex = reader.ReadByte(2);

            CustomisationCount = reader.ReadUInt();
            for (int i = 0; i < CustomisationCount; i++)
                Labels.Add(reader.ReadUInt());
            for (int i = 0; i < CustomisationCount; i++)
                Values.Add(reader.ReadUInt());

            BoneCount = reader.ReadUInt();
            for (int i = 0; i < BoneCount; i++)
                Bones.Add(reader.ReadSingle());

            UseServiceTokens = reader.ReadBit();
        }
    }
}

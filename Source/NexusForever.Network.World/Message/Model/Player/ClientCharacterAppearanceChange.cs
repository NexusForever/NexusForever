using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ClientCharacterAppearanceChange)]
    public class ClientCharacterAppearanceChange : IReadable
    {
        public Race Race { get; private set; }
        public Sex Sex { get; private set; }
        public uint CustomisationCount { get; private set; }
        public List<uint> Labels { get; private set; } = [];
        public List<uint> Values { get; private set; } = [];
        public uint BoneCount { get; private set; }
        public List<float> Bones { get; private set; } = [];
        public bool UseServiceTokens { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Race = reader.ReadEnum<Race>(5);
            Sex = reader.ReadEnum<Sex>(2);

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

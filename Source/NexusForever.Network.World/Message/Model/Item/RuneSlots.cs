using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    public class RuneSlots : IReadable, IWritable
    {
        public byte Unknown1 { get; set; }
        public byte Unknown2 { get; set; }
        public bool Unknown3 { get; set; }
        public byte[] CraftingGroup { get; set; } = new byte[8];

        public void Read(GamePacketReader reader)
        {
            uint glyphData = reader.ReadUInt();

            Unknown1 = (byte)(glyphData & 0x7);

            glyphData >>= 3;
            Unknown2 = (byte)(glyphData & 0x7);

            glyphData >>= 3;
            Unknown3 = (glyphData & 0x1) != 0;

            glyphData >>= 1;
            CraftingGroup[0] = (byte)(glyphData & 0x7);

            glyphData >>= 3;
            CraftingGroup[1] = (byte)(glyphData & 0x7);

            glyphData >>= 3;
            CraftingGroup[2] = (byte)(glyphData & 0x7);

            glyphData >>= 3;
            CraftingGroup[4] = (byte)(glyphData & 0x7);

            glyphData >>= 3;
            CraftingGroup[5] = (byte)(glyphData & 0x7);

            glyphData >>= 3;
            CraftingGroup[6] = (byte)(glyphData & 0x7);

            glyphData >>= 3;
            CraftingGroup[7] = (byte)(glyphData & 0x7);
        }

        public void Write(GamePacketWriter writer)
        {
            uint glyphdata = Unknown1 | (uint)Unknown2 << 3 | (uint)(Unknown3 ? 1 : 0) << 6;
            glyphdata |= (uint)CraftingGroup[0] << 7 | (uint)CraftingGroup[1] << 10 | (uint)CraftingGroup[2] << 13 | (uint)CraftingGroup[3] << 16;
            glyphdata |= (uint)CraftingGroup[4] << 19 | (uint)CraftingGroup[5] << 22 | (uint)CraftingGroup[6] << 25 | (uint)CraftingGroup[7] << 28;

            writer.Write(glyphdata);
        }
    }
}
